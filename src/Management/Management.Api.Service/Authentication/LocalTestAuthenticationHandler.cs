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
    public class LocalTestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private const string BEARER_PREFIX = "Bearer ";

        private readonly FirebaseApp _firebaseApp;
        private readonly TaskContext _dbContext;
        private readonly ILogger<FirebaseUserAuthenticationHandler> _logger;
        private readonly ICreateUserService _createUserService;
        private readonly IGetUserByFirebaseIdService _getUserByFirebaseId;


        public LocalTestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
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
            try
            {
                var firebaseApp = FirebaseAuth.GetAuth(_firebaseApp);
                var firebaseUser = await firebaseApp.GetUserByEmailAsync("daniel.vuust@gmail.com");
                await this.AddUserToDbIfAbsent(firebaseUser);
                return AuthenticateResult.Success(CreateAuthenticationTicket());
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex);
            }


        }
        private AuthenticationTicket CreateAuthenticationTicket()
        {
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>()
            {
                new ClaimsIdentity(ToClaims(), nameof(ClaimsIdentity))
            });

            return new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);
        }

        private IEnumerable<Claim> ToClaims()
        {
            return new List<Claim>
            {
                new Claim(FirebaseUserClaimType.ID, "oQxxPQPqHoV0Tusar0p7NTa4hIw1"),
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
