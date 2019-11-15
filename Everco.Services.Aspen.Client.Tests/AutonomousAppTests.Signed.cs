// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.Signed.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Net;
    using Everco.Services.Aspen.Client.Auth;
    using Everco.Services.Aspen.Client.Fluent;
    using Everco.Services.Aspen.Client.Providers;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Identities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticaci�n requeridas por una aplicaci�n con alcance de aut�noma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Una solicitud a una operaci�n que requiere firma cuando se usa un ApiKey no reconocido genera una respuesta inv�lida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void UnrecognizedApiKeyWhenAppSignedRequestThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            string unrecognizedApiKey = Guid.NewGuid().ToString();
            IHeadersManager apiKeyHeaderBehavior = InvalidApiKeyHeader.WithHeaderBehavior(() => unrecognizedApiKey);
            ServiceLocator.Instance.RegisterHeadersManager(apiKeyHeaderBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no v�lido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Una solicitud a una operaci�n que requiere firma cuando la aplicaci�n fue deshabilitada genera una respuesta inv�lida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void ApiKeyDisabledWhenAppSignedRequestThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.DisableApp(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey est� desactivado. P�ngase en contacto con el administrador", exception.Message);
            SqlDataContext.EnableApp(appIdentity.ApiKey);
        }

        /// <summary>
        /// Una solicitud a una operaci�n que requiere firma no hay un token de autenticaci�n vigente genera una respuesta inv�lida.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void NotFoundValidTokenWhenAppSignedRequestThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.RemoveAppAuthToken(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15847"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No hay un token de autenticaci�n vigente", exception.Message);
        }

        /// <summary>
        /// Se produce una excepci�n cuando el token de autenticaci�n proporcionado en la firma ya caduc�.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void TokenProvidedExpiredWhenAppSignedRequestThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            AuthToken authToken = client.AuthToken as AuthToken;
            Assert.That(authToken, Is.Not.Null);
            Assert.That(authToken.Expired, Is.False);
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.EnsureExpireAppAuthToken(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15848"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("El token de autenticaci�n proporcionado ya venci�", exception.Message);
        }

        /// <summary>
        /// Se produce una excepci�n de autenticaci�n cuando la aplicaci�n requiere cambiar su secreto.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void AppRequiresChangeSecretWhenUserSignedRequestThrows()
        {
            IAutonomousApp client = GetAutonomousClient();
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.EnsureAppRequiresChangeSecret(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => client.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("20009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.UpgradeRequired));
            StringAssert.IsMatch("Necesita actualizar el secreto de la aplicaci�n.", exception.Message);
            SqlDataContext.EnsureAppNotRequiresChangeSecret(appIdentity.ApiKey);
        }

        /// <summary>
        /// Se produce una excepci�n de autenticaci�n cuando se usa un token de autenticaci�n emitido por otra aplicaci�n.
        /// </summary>
        [Test]
        [Category("Signed")]
        public void MismatchSignatureWhenAppSignedRequestThrows()
        {
            IAppIdentity appMasterIdentity = AutonomousAppIdentity.Master;
            IAutonomousApp clientAppMaster = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(appMasterIdentity)
                .AuthenticateNoCache()
                .GetClient();

            Assert.That(clientAppMaster, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken, Is.Not.Null);
            Assert.That(clientAppMaster.AuthToken.Token, Is.Not.Null);

            IAppIdentity appHelperIdentity = AutonomousAppIdentity.Helper;
            IAutonomousApp clientAppHelper = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(appHelperIdentity)
                .AuthenticateNoCache()
                .GetClient();

            Assert.That(clientAppHelper, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken, Is.Not.Null);
            Assert.That(clientAppHelper.AuthToken.Token, Is.Not.Null);

            IPayloadClaimsManager mismatchTokenClaimBehavior = InvalidTokenPayloadClaim.WithClaimBehavior(() => clientAppHelper.AuthToken.Token);
            ServiceLocator.Instance.RegisterPayloadClaimsManager(mismatchTokenClaimBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => clientAppMaster.Settings.GetDocTypes());
            Assert.That(exception.EventId, Is.EqualTo("15846"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            StringAssert.IsMatch("No coinciden los datos recibidos del token vs los valores esperados. �Se modificaron los valores en tr�nsito o est� utilizando el ApiKey en otra aplicaci�n?", exception.Message);
        }
    }
}