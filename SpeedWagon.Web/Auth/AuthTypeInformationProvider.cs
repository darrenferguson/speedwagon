using SpeedWagon.Web.Enum;
using SpeedWagon.Web.Interfaces;

namespace SpeedWagon.Web.Auth
{
    public class AuthTypeInformationProvider : IAuthTypeInformationProvider
    {
        private readonly AuthType _authType;

        public AuthTypeInformationProvider(AuthType authType)
        {
            this._authType = authType;
        }

        public AuthType GetAuthType()
        {
            return this._authType;
        }
    }
}
