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
using SpeedWagon.Runtime.Interfaces;
using SpeedWagon.Runtime.Models;
using Directory = Lucene.Net.Store.Directory;

namespace SpeedWagon.Runtime.Services
{
    public class LuceneSearchService : ISearchService
    {
        private readonly IContentService _contentService;

        private readonly string _directory;

        private Analyzer SetupAnalyzer() =>
            new StandardAnalyzer(LuceneVersion.LUCENE_48);

        public LuceneSearchService(IContentService contentService, string directory)
        {



            this._contentService = contentService;
            this._contentService.Added += ContentServiceAdded;
            this._contentService.Removed += ContentServiceRemoved;
            this._directory = directory;
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

        private string SafeFieldValue(string v)
        {
            if (!string.IsNullOrEmpty(v))
            {
                return v;
            }

            return string.Empty;
        }

        private Document GetLuceneDocument(SpeedWagonContent content)
        {
            var d = new Document();

            d.Add(new Field("Url", content.Url, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new TextField("Name", content.Name, Field.Store.YES));

            d.Add(new Field("CreateDate", content.CreateDate.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("UpdateDate", content.UpdateDate.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            d.Add(new Field("Type", content.Type, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("CreatorName", content.CreatorName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("WriterName", content.WriterName, Field.Store.YES, Field.Index.NOT_ANALYZED));

            d.Add(new Field("RelativeUrl", content.RelativeUrl, Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Template", SafeFieldValue(content.Template), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("SortOrder", content.SortOrder.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            d.Add(new Field("Level", content.Level.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            foreach (var property in content.Content)
            {
                var value = property.Value == null ? string.Empty : property.Value.ToString();
                value = StripHtml(value);
                string key = property.Key.Replace(" ", "");
                d.Add(new TextField(key, value, Field.Store.YES));
            }

            return d;
        }

        protected string StripHtml(string htmlString)
        {
            const string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlString, pattern, string.Empty);
        }



        public void Index(SpeedWagonContent model)
        {
            var doc = GetLuceneDocument(model);
            using (Directory directory = FSDirectory.Open(this._directory))
            {
                using (IndexWriter writer = new IndexWriter(directory, new IndexWriterConfig(LuceneVersion.LUCENE_48, SetupAnalyzer())))
                {
                    writer.DeleteDocuments(new Term("Url", doc.Get("Url")));
                    writer.Commit();

                    writer.AddDocument(doc);
                    writer.Commit();
                }
            }
        }

        private string GeneratePreviewText(Query q, string text)
        {
            return text;
            // var scorer = new QueryScorer(q);
            //var formatter = new SimpleHTMLFormatter("<em>", "</em>");

            //var highlighter = new Highlighter(formatter, scorer);

            //highlighter.TextFragmenter = new SimpleFragmenter(250);

            //var stream = new StandardAnalyzer(Version.LUCENE_CURRENT).TokenStream("bodyText", new StringReader(text));
            //return highlighter.GetBestFragments(stream, text, 3, "...");
        }

        public void Delete(string url)
        {
            using (Directory directory = FSDirectory.Open(this._directory))
            {
                using (var writer = new IndexWriter(directory,
                    new IndexWriterConfig(LuceneVersion.LUCENE_48, SetupAnalyzer())))
                {
                    writer.DeleteDocuments(new Term("Url", url));
                    writer.Commit();
                }
            }
        }

        private QueryParser SetupQueryParser(Analyzer analyzer)
        {
            return new MultiFieldQueryParser(
            LuceneVersion.LUCENE_48,
            new[] { "BodyText", "Description" },
            analyzer
            );
        }

        public async Task<IEnumerable<SearchResult>> Search(string searchTerm)
        {
            var results = new List<SearchResult>();

            var queryParser = SetupQueryParser(SetupAnalyzer());
            var query = queryParser.Parse(searchTerm);

            ScoreDoc[] scored;

            using (Directory directory = FSDirectory.Open(this._directory))
            {
                SearcherManager searchManager = new SearcherManager(directory, null);

                searchManager.MaybeRefreshBlocking();
                IndexSearcher indexSearcher = searchManager.Acquire();

                TopDocs hits = indexSearcher.Search(query, int.MaxValue);
                
                scored = hits.ScoreDocs;

                foreach (var item in scored)
                {
                    Document doc = indexSearcher.Doc(item.Doc);
                    var previewText = GeneratePreviewText(query, doc.Get("bodyText"));
                    string url = doc.Get("Url");
                    var content = await this._contentService.GetContent(url);

                    results.Add(new SearchResult
                    {
                        Url = url,
                        PreviewText = doc.Get("BodyText"),
                        Score = item.Score,
                        Content = content
                        
                    });
                }
            }

            return results;
        }

        public Task<IEnumerable<string>> Search(IDictionary<string, string> matches)
        {
            var results = new List<string>();
            using (Directory directory = FSDirectory.Open(this._directory))
            {
                SearcherManager searchManager = new SearcherManager(directory, null);

                searchManager.MaybeRefreshBlocking();
                IndexSearcher indexSearcher = searchManager.Acquire();

                var booleanQuery = new BooleanQuery();

                foreach (var match in matches)
                {
                    booleanQuery.Add(new TermQuery(new Term(match.Key, match.Value)), Occur.MUST);
                }

                var hits = indexSearcher.Search(booleanQuery, int.MaxValue);
                var scored = hits.ScoreDocs;

                foreach (var item in scored)
                {
                    Document doc = indexSearcher.Doc(item.Doc);
                    var url = doc.Get("Url");
                    results.Add(url);
                }
            }

            return Task.FromResult<IEnumerable<string>>(results);
        }
    }
}