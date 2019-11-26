// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.Signin.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Everco.Services.Aspen.Client.Tests.Assets;
    using Fluent;
    using Identities;
    using Identity;
    using NUnit.Framework;
    using Providers;
    using IAppIdentity = Auth.IAppIdentity;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticaci�n requeridas por una aplicaci�n con alcance de aut�noma.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Autenticar una aplicaci�n aut�noma funciona.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void AppSigninRequestWorks()
        {
            IAutonomousApp client = this.GetAutonomousClient();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Se produce una excepci�n de autenticaci�n si la aplicaci�n no es reconocida en el sistema.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void UnrecognizedApiKeyThrows()
        {
            string randomApiKey = Guid.NewGuid().ToString();
            string apiKeySecret = AutonomousAppIdentity.Master.ApiSecret;
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(randomApiKey, apiKeySecret)
                        .AuthenticateNoCache()
                        .GetClient();
                });

            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no v�lido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Se produce una excepci�n de autenticaci�n si la credencial de la aplicaci�n no es v�lida.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void InvalidAppCredentialThrows()
        {
            string recognizedApiKey = AutonomousAppIdentity.Master.ApiKey;
            string invalidApiSecret = Guid.NewGuid().ToString();
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(recognizedApiKey, invalidApiSecret)
                        .AuthenticateNoCache()
                        .GetClient();
                });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        /// <summary>
        /// Autenticar una aplicaci�n cuando se usa un ApiKey con alcance de delegada no funciona.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void ApiKeyScopeMismatchThrows()
        {
            IAppIdentity delegatedAppIdentity = DelegatedAppIdentity.Master;
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(delegatedAppIdentity)
                        .AuthenticateNoCache()
                        .GetClient();
                });
            Assert.That(exception.EventId, Is.EqualTo("1000478"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey no tiene permisos para realizar la operaci�n. Alcance requerido: 'Autonomous'", exception.Message);
        }

        /// <summary>
        /// Se produce una excepci�n de autenticaci�n cuando la aplicaci�n esta inhabilitada.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void ApiKeyDisabledWhenAppSigninRequestThrows()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.DisableApp(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => GetAutonomousClient());
            Assert.That(exception.EventId, Is.EqualTo("20006"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey est� desactivado. P�ngase en contacto con el administrador", exception.Message);
            SqlDataContext.EnableApp(appIdentity.ApiKey);
        }

        /// <summary>
        /// Se produce una excepci�n de autenticaci�n cuando la aplicaci�n requiere cambiar su secreto.
        /// </summary>
        [Test]
        [Category("Signin")]
        public void AppRequiresChangeSecretWhenSigninRequestThrows()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            SqlDataContext.EnsureAppRequiresChangeSecret(appIdentity.ApiKey);
            AspenException exception = Assert.Throws<AspenException>(() => GetAutonomousClient());
            Assert.That(exception.EventId, Is.EqualTo("20009"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.UpgradeRequired));
            StringAssert.IsMatch("Necesita actualizar el secreto de la aplicaci�n.", exception.Message);
            SqlDataContext.EnsureAppNotRequiresChangeSecret(appIdentity.ApiKey);
        }

        [Test]
        public void SecureIdentityWorks()
        {
            SecureIdentity identity = new SecureIdentity();
            List<string> input = new List<string>();
            for (int index = 0; index < 5; index++)
            {
                input.Add(Guid.NewGuid().ToString("P"));
            }

            string clearText = string.Join(' ', input);
            string encryptedText = identity.Encrypt(clearText);
            string result = identity.Decrypt(encryptedText);
            Assert.That(result, Is.EqualTo(clearText));
        }

        [Test]
        public void SecureLongTextIdentityWorks()
        {
            string clearText = TestContext.CurrentContext.Random.GetString(500);
            SecureIdentity identity = new SecureIdentity();
            string encryptedText = identity.Encrypt(clearText);
            string result = identity.Decrypt(encryptedText);
            Assert.That(result, Is.EqualTo(clearText));
        }
    }
}