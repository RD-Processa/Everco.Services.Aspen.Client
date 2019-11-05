// -----------------------------------------------------------------------
// <copyright file="Autonomous.SigninHeaders.Tests.cs" company="Evertec Colombia">
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
    using Assets;
    using Fluent;
    using Identities;
    using NUnit.Framework;
    using Providers;

    /// <summary>
    /// Implementa las pruebas unitarias de las cabeceras de autenticaci�n requeridas por una aplicaci�n con alcance de aut�noma.
    /// </summary>
    [TestFixture]
    public class AutonomousSigninHeadersTests
    {
        /// <summary>
        /// Proporciona un conjunto com�n de funciones que se ejecutar�n antes de llamar a cada m�todo de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        /// <summary>
        /// Autenticar una aplicaci�n funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void SigninRequestWorks()
        {
            IAutonomousApp client = AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .Authenticate()
                    .GetClient();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void SigninUsingApiKeyDelegatedThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                IAutonomousApp client = AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("1000478"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey no tiene permisos para realizar la operaci�n. Alcance requerido: 'Autonomous'", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void InvalidApiKeyThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                string randomApiKey = Guid.NewGuid().ToString();
                string apiKeySecret = AutonomousAppIdentity.Default.ApiSecret;
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(randomApiKey, apiKeySecret)
                    .Authenticate()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no v�lido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void InvalidApiSecretThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                string apiKey = AutonomousAppIdentity.Default.ApiKey;
                string randomApiSecret = Guid.NewGuid().ToString();
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(apiKey, randomApiSecret)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        /// <summary>
        /// Autenticar una aplicaci�n cuando falta el encabezado del ApiKey no funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void MissingApiKeyHeaderThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(new MissingApiKeyHeader());
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Nulls the or empty API key header throws.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void NullOrEmptyApiKeyHeaderThrows()
        {
            IList<IHeadersManager> apiKeyHeaderBehaviors = new List<IHeadersManager>()
            {
                new MissingApiKeyHeader(() => null),
                new MissingApiKeyHeader(() => string.Empty),
                new MissingApiKeyHeader(() => "      ")
            };

            foreach (IHeadersManager behavior in apiKeyHeaderBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void MissingPayloadHeaderThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(new MissingPayloadHeader());
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void NullOrEmptyPayloadHeaderThrows()
        {
            IList<IHeadersManager> payloadHeaderBehaviors = new List<IHeadersManager>()
            {
                new MissingPayloadHeader(() => null),
                new MissingPayloadHeader(() => string.Empty),
                new MissingPayloadHeader(() => "     ")
            };

            foreach (IHeadersManager headerBehavior in payloadHeaderBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void InvalidPayloadSignatureThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(new MissingPayloadHeader(() => "Lorem ipsum dolor sit amet, consetetur sadipscing elitr"));
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void MissingNonceThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(new MissingNoncePayloadHeader());
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void NullOrEmptyNonceThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingNoncePayloadHeader(HeaderValueBehavior.Null),
                new MissingNoncePayloadHeader(HeaderValueBehavior.Empty),
                new MissingNoncePayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void InvalidNonceFormatThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingNoncePayloadHeader(HeaderValueBehavior.UnexpectedFormat),
                new MissingNoncePayloadHeader(HeaderValueBehavior.MaxLengthExceeded)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patr�n", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void NonceAlreadyProcessedThrows()
        {
            DuplicatedNonceGenerator nonceGenerator = new DuplicatedNonceGenerator();
            ServiceLocator.Instance.RegisterNonceGenerator(nonceGenerator);
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .AuthenticateNoCache()
                .GetClient();

            // Se puede autenticar la aplicaci�n usando el nonce la primera vez.
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);

            // No se podr� autenticar la aplicaci�n, cuando use el mismo nonce por segunda vez.
            Assert.AreEqual(ServiceLocator.Instance.NonceGenerator.GetNonce(), nonceGenerator.GetNonce());
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicaci�n", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void MissingEpochThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(new MissingEpochPayloadHeader());
                AutonomousApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache()
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void NullOrEmptyEpochThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingEpochPayloadHeader(HeaderValueBehavior.Null),
                new MissingEpochPayloadHeader(HeaderValueBehavior.Empty),
                new MissingEpochPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void InvalidEpochFormatThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingEpochPayloadHeader(HeaderValueBehavior.UnexpectedFormat),
                new MissingEpochPayloadHeader(HeaderValueBehavior.MinLengthRequired),
                new MissingEpochPayloadHeader(HeaderValueBehavior.MaxLengthExceeded)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un n�mero.", exception.Message);
            }
        }

        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void EpochExpiredThrows()
        {

            int randomDays = new Random().Next(5, 10);
            IList<IEpochGenerator> epochBehaviors = new List<IEpochGenerator>()
            {
                new DatePickerEpochGenerator(-randomDays),
                new DatePickerEpochGenerator(randomDays),
            };

            foreach (IEpochGenerator behavior in epochBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterEpochGenerator(behavior);
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15851"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
                StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicaci�n sin la cabecera de la versi�n del API solicitada funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void MissingApiVersionHeaderWorks()
        {
            ServiceLocator.Instance.RegisterHeadersManager(new MissingApiVersionHeader());
            IAutonomousApp client = AutonomousApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(AutonomousAppIdentity.Default)
                .Authenticate()
                .GetClient();

            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores nulos o vac�os en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void NullEmptyApiVersionHeaderThrows()
        {
            IList<IHeadersManager> apiVersionHeaderBehaviors = new List<IHeadersManager>()
            {
                new MissingApiVersionHeader(() => null),
                new MissingApiVersionHeader(() => string.Empty),
                new MissingApiVersionHeader(() => "     ")
            };

            foreach (IHeadersManager headerBehavior in apiVersionHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato v�lido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores inv�lidos en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void InvalidApiVersionHeaderFormatThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new MissingApiVersionHeader(() => "abc"),
                new MissingApiVersionHeader(() => Guid.NewGuid().ToString()),
                new MissingApiVersionHeader(() => "123"),
                new MissingApiVersionHeader(() => "1,0"),
                new MissingApiVersionHeader(() => "1A"),
                new MissingApiVersionHeader(() => "-1.0"),

            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato v�lido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores no soportados en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Autonomous.Signin.Headers")]
        public void UnsupportedApiVersionHeaderThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new MissingApiVersionHeader(() => "0.1"),
                new MissingApiVersionHeader(() => "999999.999999"),
                new MissingApiVersionHeader(() => "999999.999999.999999"),
                new MissingApiVersionHeader(() => "999999.999999.999999.999999")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    AutonomousApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(AutonomousAppIdentity.Default)
                        .AuthenticateNoCache()
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99005"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un valor admitido para el encabezado personalizado 'X-PRO-Api-Version'", exception.Message);
            }
        }
    }
}