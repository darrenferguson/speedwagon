using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using SpeedWagon.Interfaces;
using SpeedWagon.Models;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers.Classic;

namespace SpeedWagon.Runtime.Services
{
    public class LuceneSearchService : ISearchService
    {
        private readonly IContentService _contentService;
        private readonly object _lock;

        private readonly RAMDirectory _directory;
        private readonly IndexWriter _writer;

        private Analyzer SetupAnalyzer() => 
            new StandardAnalyzer(LuceneVersion.LUCENE_48, StandardAnalyzer.STOP_WORDS_SET);

        public LuceneSearchService(IContentService contentService)
        {
            _lock = new object();

           
            _directory = new RAMDirectory();

            //_writer = new IndexWriter(_directory, new StandardAnalyzer(_version), IndexWriter.MaxFieldLength.UNLIMITED);

            this._contentService = contentService;
            this._contentService.Added += ContentServiceAdded;
            this._contentService.Removed += ContentServiceRemoved;

        }

        private void ContentServiceRemoved(string sender, EventArgs e)
        {
            Delete(sender);
        }

        private void ContentServiceAdded(SpeedWagonContent sender, EventArgs e)
        {
            Index(sender);
        }

        public async Task IndexAll(IContentService contentService)
        {
            var urls = contentService.GetUrlList();

            // BIG TODO: make search index in the background or on demand.
            // Forces everything to be cached on startup... :(

            foreach (var url in urls.ToList())
            {
                var content = await contentService.GetContent(url);

                if (content != null)
                    Index(content);
            }
        }

        private Document GetLuceneDocument(SpeedWagonContent content)
        {
            var d = new Document();

            d.Add(new Field("Url", content.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Name", content.Name, Field.Store.YES, Field.Index.ANALYZED));

            d.Add(new Field("CreateDate", content.CreateDate.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("UpdateDate", content.UpdateDate.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            d.Add(new Field("Type", content.Type, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("CreatorName", content.CreatorName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("WriterName", content.WriterName, Field.Store.YES, Field.Index.NOT_ANALYZED));

            d.Add(new Field("RelativeUrl", content.RelativeUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Template", content.Template, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("SortOrder", content.SortOrder.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Level", content.Level.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            foreach (var property in content.Content)
            {
                var value = property.Value.ToString();
                value = StripHtml(value);

                d.Add(new Field(property.Key, value, Field.Store.YES, Field.Index.ANALYZED));
            }

            return d;
        }

        protected string StripHtml(string htmlString)
        {
            const string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }

        // For debugging!

        //public void FlushToDisc()
        //{
        //    var dir = new DirectoryInfo(@"C:\temp\index");
        //    foreach (var file in dir.GetFiles())
        //    {
        //        file.Delete();
        //    }

        //    var targetDir = FSDirectory.Open(dir);

        //    Directory.Copy(_directory, targetDir, false);
        //}

        public void Index(SpeedWagonContent model)
        {
            var doc = GetLuceneDocument(model);

            using (var writer = new IndexWriter(_directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, SetupAnalyzer())))
            {


                writer.DeleteDocuments(new Term("Url", doc.Get("Url")));
                writer.Commit();

                writer.AddDocument(doc);
                writer.Commit();

                    //if (Logger.IsDebugEnabled)
                    //{
                    //    Logger.Debug("Indexing: " + model.Name + " for search.");
                    //    // FlushToDisc();
                    //}
                
               
            }

            
        }


        private string GeneratePreviewText(Query q, string text)
        {
            return text;
            //var scorer = new QueryScorer(q);
            //var formatter = new SimpleHTMLFormatter("<em>", "</em>");

            //var highlighter = new Highlighter(formatter, scorer);

            //highlighter.TextFragmenter = new SimpleFragmenter(250);

            //var stream = new StandardAnalyzer(Version.LUCENE_CURRENT).TokenStream("bodyText", new StringReader(text));
            //return highlighter.GetBestFragments(stream, text, 3, "...");
        }

        public void Delete(string url)
        {
            using (var writer = new IndexWriter(_directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, SetupAnalyzer())))
            {
                try
                {
                    writer.DeleteDocuments(new Term("Url", url));
                    writer.Commit();
                }
                catch (Exception ex)
                {
                    //Logger.Warn(ex);
                }
                finally
                {
                    // writer.Optimize();
                    writer.Dispose();
                }
            }
        }

        private QueryParser SetupQueryParser(Analyzer analyzer)
        {
            return new MultiFieldQueryParser(
            LuceneVersion.LUCENE_48,
            new[] { "bodyText", "description" },
            analyzer
            );
        }

        public Task<IEnumerable<SearchResult>> Search(string searchTerm)
        {
            var results = new List<SearchResult>();


            SearcherManager searchManager = new SearcherManager(_directory, null);

            searchManager.MaybeRefreshBlocking();
            IndexSearcher indexSearcher = searchManager.Acquire();



            try
            {
                var queryParser = SetupQueryParser(SetupAnalyzer());

                var query = queryParser.Parse(searchTerm);
                var hits = indexSearcher.Search(query, int.MaxValue);
                var scored = hits.ScoreDocs;


                foreach (var item in scored)
                {
                    Document doc = indexSearcher.Doc(item.Doc);
                    var previewText = GeneratePreviewText(query, doc.Get("bodyText"));

                    results.Add(new SearchResult
                    {
                        Url = doc.Get("Url"),
                        PreviewText = previewText

                    });
                }
            }
            catch (Exception ex)
            {
                //Logger.Warn(ex);
            }
            finally
            {
                // indexSearcher.Close();
            }

            return Task.FromResult<IEnumerable<SearchResult>>(results);
        }

        public Task<IEnumerable<string>> Search(IDictionary<string, string> matches)
        {
            var results = new List<string>();
            SearcherManager searchManager = new SearcherManager(_directory, null);

            searchManager.MaybeRefreshBlocking();
            IndexSearcher indexSearcher = searchManager.Acquire();

            var booleanQuery = new BooleanQuery();

            foreach (var match in matches)
            {
                booleanQuery.Add(new TermQuery(new Term(match.Key, match.Value)), Occur.MUST);
            }

            try
            {
                var hits = indexSearcher.Search(booleanQuery, int.MaxValue);
                var scored = hits.ScoreDocs;

                foreach (var item in scored)
                {
                    Document doc = indexSearcher.Doc(item.Doc);
                    var url = doc.Get("Url");
                    results.Add(url);
                }
            }
            catch (Exception ex)
            {
                //Logger.Warn(ex);
            }
            finally
            {
                //indexSearcher.Close();
                //indexSearcher.Dispose();
            }

            return Task.FromResult<IEnumerable<string>>(results);
        }
    }
}