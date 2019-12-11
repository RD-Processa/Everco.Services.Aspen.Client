// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.UpdateApiSecret.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System;
    using System.Net;
    using Everco.Services.Aspen.Client.Fluent;
    using Identities;
    using Identity;
    using NUnit.Framework;
    using PasswordGenerator;

    /// <summary>
    /// Implementa las pruebas unitarias de la solicitud de actualizaci�n del secreto de la aplicaci�n.
    /// </summary>
    [TestFixture]
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Actualizar el secreto de una aplicaci�n produce una salida exitosa.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void UpdateApiSecretRequestWorks()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            string apiKey = appIdentity.ApiKey;
            string currentApiSecret = appIdentity.ApiSecret;
            string newApiSecret = this.GetRandomSecret();

            Assert.That(newApiSecret.Length, Is.GreaterThanOrEqualTo(128));
            Assert.DoesNotThrow(() => AutonomousApp.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(apiKey, currentApiSecret)
                .UpdateApiSecret(newApiSecret));

            TestContext.CurrentContext.DatabaseHelper().UpdateApiSecret(apiKey, currentApiSecret);
        }

        /// <summary>
        /// Luego de la actualizaci�n del secreto de la aplicaci�n, se espera que el formato del secreto sea 'SecretFormat.Encrypted' (1)
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void SecretFormatMustBeEncryptedTypeAfterUpdate()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            string apiKey = appIdentity.ApiKey;
            string currentApiSecret = appIdentity.ApiSecret;
            string newApiSecret = this.GetRandomSecret();

            Assert.DoesNotThrow(() => AutonomousApp.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(apiKey, currentApiSecret)
                .UpdateApiSecret(newApiSecret));

            // El tipo de formato del secreto debe ser 'SecretFormat.Encrypted' (1)
            Assert.IsTrue(TestContext.CurrentContext.DatabaseHelper().AppSecretFormatIsEncrypted(apiKey));

            TestContext.CurrentContext.DatabaseHelper().UpdateApiSecret(apiKey, currentApiSecret);
        }

        /// <summary>
        /// Luego de cambiar el secreto de una aplicaci�n, la nueva credencial produce un token de autenticaci�n v�lido.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void UseNewSecretWhenAppSigninRequestWorks()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            string apiKey = appIdentity.ApiKey;
            string currentApiSecret = appIdentity.ApiSecret;
            string newApiSecret = this.GetRandomSecret();

            Assert.DoesNotThrow(() => AutonomousApp.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(apiKey, currentApiSecret)
                .UpdateApiSecret(newApiSecret));

            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(apiKey, newApiSecret)
                .AuthenticateNoCache()
                .GetClient();

            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);

            TestContext.CurrentContext.DatabaseHelper().UpdateApiSecret(apiKey, currentApiSecret);
        }

        /// <summary>
        /// Luego de cambiar el secreto de una aplicaci�n, cuando se usa el secreto anterior produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void UseOlderSecretWhenAppSigninRequestThrows()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            string apiKey = appIdentity.ApiKey;
            string currentApiSecret = appIdentity.ApiSecret;
            string newApiSecret = new Password(128).Next();

            Assert.DoesNotThrow(() => AutonomousApp.Initialize()
                .RoutingTo(TestingEndpointProvider.Default)
                .WithIdentity(apiKey, currentApiSecret)
                .UpdateApiSecret(newApiSecret));

            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(apiKey, currentApiSecret)
                        .AuthenticateNoCache()
                        .GetClient();
                });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido. Invalid signature. �Est� utilizando las credenciales proporcionadas?", exception.Message);

            TestContext.CurrentContext.DatabaseHelper().UpdateApiSecret(apiKey, currentApiSecret);
        }

        /// <summary>
        /// Cuando el secreto actual es igual al nuevo secreto se produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void NewAndOlderApiSecretAreEqualsThrows()
        {
            IAppIdentity appIdentity = AutonomousAppIdentity.Master;
            string apiKey = appIdentity.ApiKey;
            string currentApiSecret = appIdentity.ApiSecret;
            string randomApiSecret = this.GetRandomSecret();

            // Ya que el secreto actual de la aplicaci�n de pruebas s�lo tiene letras,
            // para saltar las validaciones del nuevo secreto se establece en la aplicaci�n,
            // un secreto aleatorio y a as� validar que el nuevo secreto sea igual al actual.
            TestContext.CurrentContext.DatabaseHelper().UpdateApiSecret(apiKey, randomApiSecret);

            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(apiKey, randomApiSecret)
                        .UpdateApiSecret(randomApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15864"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("El nuevo secreto no debe ser igual al actual.", exception.Message);

            // Se reestablece el secreto predeterminado.
            TestContext.CurrentContext.DatabaseHelper().UpdateApiSecret(apiKey, currentApiSecret);
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n es nulo o vac�o produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void NullOrEmptyNewApiSecretThrows()
        {
            string[] invalidNewApiSecrets = { null, string.Empty, "    " };
            foreach (string newApiSecret in invalidNewApiSecrets)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                    {
                        AutonomousApp.Initialize()
                            .RoutingTo(TestingEndpointProvider.Default)
                            .WithIdentity(AutonomousAppIdentity.Master)
                            .UpdateApiSecret(newApiSecret);
                    });

                Assert.That(exception.EventId, Is.EqualTo("15864"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
                StringAssert.IsMatch("'NewValue' no deber�a estar vac�o.", exception.Message);
            }
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n tiene una longitud menor a 128 caracteres produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void InvalidMinLengthNewApiSecretThrows()
        {
            int minLengthAllowed = 128;
            int randomMinLength = new Random().Next(10, minLengthAllowed - 1);
            string invalidNewApiSecret = this.GetRandomSecret(randomMinLength);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .UpdateApiSecret(invalidNewApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15864"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("'NewValue' debe ser mayor o igual que 128 caracteres", exception.Message);
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n tiene una longitud mayor a 214 caracteres produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void InvalidMaxLengthNewApiSecretThrows()
        {
            int maxLengthAllowed = 214;
            int randomMaxLength = new Random().Next(maxLengthAllowed + 1, maxLengthAllowed * 2);
            string invalidNewApiSecret = this.GetRandomSecret(randomMaxLength);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .UpdateApiSecret(invalidNewApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15892"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("El nuevo secreto excede la longitud m�xima permitida", exception.Message);
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n no incluye letras may�sculas produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void NotIncludeUppercaseLettersNewApiSecretThrows()
        {
            string invalidNewApiSecret = this.GetRandomSecret(includeUppercase: false);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .UpdateApiSecret(invalidNewApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15864"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("Utilice secretos complejos para mantener su aplicaci�n segura. Combine may�sculas y min�sculas, n�meros y caracteres especiales", exception.Message);
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n no incluye letras min�sculas produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void NotIncludeLowercaseLettersNewApiSecretThrows()
        {
            string invalidNewApiSecret = this.GetRandomSecret(includeLowercase: false);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .UpdateApiSecret(invalidNewApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15864"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("Utilice secretos complejos para mantener su aplicaci�n segura. Combine may�sculas y min�sculas, n�meros y caracteres especiales", exception.Message);
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n no incluye n�meros produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void NotIncludeNumericNewApiSecretThrows()
        {
            string invalidNewApiSecret = this.GetRandomSecret(includeNumeric: false);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .UpdateApiSecret(invalidNewApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15864"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("Utilice secretos complejos para mantener su aplicaci�n segura. Combine may�sculas y min�sculas, n�meros y caracteres especiales", exception.Message);
        }

        /// <summary>
        /// Cuando el nuevo secreto para la aplicaci�n no incluye caracteres especiales produce una salida inv�lida.
        /// </summary>
        [Test]
        [Category("Signin.UpdateApiSecret")]
        public void NotIncludeSpecialCharsNewApiSecretThrows()
        {
            string invalidNewApiSecret = this.GetRandomSecret(includeSpecial: false);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(TestingEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Master)
                        .UpdateApiSecret(invalidNewApiSecret);
                });

            Assert.That(exception.EventId, Is.EqualTo("15864"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.NotAcceptable));
            StringAssert.IsMatch("Utilice secretos complejos para mantener su aplicaci�n segura. Combine may�sculas y min�sculas, n�meros y caracteres especiales", exception.Message);
        }

        /// <summary>
        /// Obtiene un secreto aleatorio para una aplicaci�n.
        /// </summary>
        /// <param name="length">La longitud requerida.</param>
        /// <param name="includeLowercase">Si se establece en <c>true</c>, se incluir�n las letras min�sculas.</param>
        /// <param name="includeUppercase">Si se establece en <c>true</c>, se incluir�n las letras may�sculas.</param>
        /// <param name="includeNumeric">Si se establece en <c>true</c>, se incluir�n numeros.</param>
        /// <param name="includeSpecial">Si se establece en <c>true</c>, se incluir�n caracteres especiales.</param>
        /// <returns>Una cadena generada aleatoriamente que representa el secreto para una aplicaci�n.</returns>
        public string GetRandomSecret(
            int length = 128,
            bool includeLowercase = true,
            bool includeUppercase = true,
            bool includeNumeric = true,
            bool includeSpecial = true)
        {
            string randomSecret = null;
            Password passwordGenerator = new Password(
                includeLowercase,
                includeUppercase,
                includeNumeric,
                includeSpecial,
                128);

            do
            {
                randomSecret += passwordGenerator.Next();
            }
            while (randomSecret.Length < length);
            return randomSecret.Substring(0, length);
        }
    }
}