using ELTEKAps.Management.ApplicationServices.Users.Create;
using ELTEKAps.Management.ApplicationServices.Users.Get;
using ELTEKAps.Management.Infrastructure.Repositories.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace ELTEKAps.Management.Api.Service.Authentication
{
    public class FirebaseUserAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string BEARER_PREFIX = "Bearer ";

        private readonly FirebaseApp _firebaseApp;
        private readonly TaskContext _dbContext;
        private readonly ILogger<FirebaseUserAuthenticationHandler> _logger;
        private readonly ICreateUserService _createUserService;
        private readonly IGetUserByFirebaseIdService _getUserByFirebaseId;


        public FirebaseUserAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            FirebaseApp firebaseApp,
            TaskContext dbContext, ICreateUserService createUserService, IGetUserByFirebaseIdService getUserByFirebaseId)
            : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
            _dbContext = dbContext;
            _createUserService = createUserService;
            _getUserByFirebaseId = getUserByFirebaseId;
            this._logger = logger.CreateLogger<FirebaseUserAuthenticationHandler>();
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            this._logger.LogInformation("Calling HandleAuthenticateAsync for FirebaseUserAuthenticationHandler");
            if (!Context.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No authorization");
            }

            string? bearerToken = Context.Request.Headers["Authorization"];

            if (bearerToken == null || !bearerToken.StartsWith(BEARER_PREFIX))
            {
                return AuthenticateResult.Fail("Invalid scheme.");
            }

            string token = bearerToken.Substring(BEARER_PREFIX.Length);

            try
            {
                var firebaseApp = FirebaseAuth.GetAuth(_firebaseApp);
                FirebaseToken firebaseToken = await firebaseApp.VerifyIdTokenAsync(token);
                var firebaseUser = await firebaseApp.GetUserAsync(firebaseToken.Uid);
                await this.AddUserToDbIfAbsent(firebaseUser);

                return AuthenticateResult.Success(CreateAuthenticationTicket(firebaseToken));
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }
        }

        private AuthenticationTicket CreateAuthenticationTicket(FirebaseToken firebaseToken)
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(ClaimsIdentity))
            });

            return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        }

        private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            return new List<Claim>
            {
                new Claim(FirebaseUserClaimType.ID, claims.GetValueOrDefault("user_id", "").ToString()!),
                new Claim(FirebaseUserClaimType.EMAIL, claims.GetValueOrDefault("email", "").ToString()!),
                new Claim(FirebaseUserClaimType.EMAIL_VERIFIED, claims.GetValueOrDefault("email_verified", "").ToString()!),
                new Claim(FirebaseUserClaimType.USERNAME, claims.GetValueOrDefault("name", "").ToString()!),
            };
        }

        private async Task AddUserToDbIfAbsent(UserRecord user)
        {

            var newUser = await _getUserByFirebaseId.GetUserByFireBaseId(user.Uid);
            if (newUser == null)
            {
                await this._createUserService.RequestCreateUser(user.Uid, user.Email, user.DisplayName ?? user.Email);
            }
            else if (newUser.Name != (user.DisplayName ?? user.Email))
            {
                await this._createUserService.UpdateDisplayName(user.Uid, user.DisplayName);
            }
        }
    }
}
