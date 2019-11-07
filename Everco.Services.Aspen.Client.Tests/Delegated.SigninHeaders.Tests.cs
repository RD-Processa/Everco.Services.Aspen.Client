// -----------------------------------------------------------------------
// <copyright file="Delegated.SigninHeaders.Tests.cs" company="Evertec Colombia">
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
    /// Implementa las pruebas unitarias de las cabeceras de autenticaci�n requeridas por una aplicaci�n con alcance de delegada.
    /// </summary>
    [TestFixture]
    public class DelegatedSigninHeadersTests
    {
        /// <summary>
        /// Proporciona un conjunto com�n de funciones que se ejecutar�n antes de llamar a cada m�todo de prueba.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void SigninRequestWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void SigninUsingApiKeyDelegatedThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("1000478"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
            StringAssert.IsMatch("ApiKey no tiene permisos para realizar la operaci�n. Alcance requerido: 'Delegated'", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidApiKeyThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                string randomApiKey = Guid.NewGuid().ToString();
                string apiKeySecret = AutonomousAppIdentity.Default.ApiSecret;
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(randomApiKey, apiKeySecret)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20005"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Identificador de ApiKey no v�lido para la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidApiSecretThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                string apiKey = AutonomousAppIdentity.Default.ApiKey;
                string randomApiSecret = Guid.NewGuid().ToString();
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(apiKey, randomApiSecret)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingApiKeyHeaderThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(InvalidApiKeyHeader.AvoidingHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyApiKeyHeaderThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiKeyHeader.WithHeaderBehavior(() => null),
                InvalidApiKeyHeader.WithHeaderBehavior(() => string.Empty),
                InvalidApiKeyHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingPayloadHeaderThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(InvalidPayloadHeader.AvoidingHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyPayloadHeaderThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                InvalidPayloadHeader.WithHeaderBehavior(() => null),
                InvalidPayloadHeader.WithHeaderBehavior(() => string.Empty),
                InvalidPayloadHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterHeadersManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
            }
        }

        /// <summary>
        /// Cuando el valor del payload no es un jwt no funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidPayloadSignatureThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterHeadersManager(InvalidPayloadHeader.WithHeaderBehavior(() => "Lorem ipsum dolor sit amet, consetetur sadipscing elitr"));
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void MissingNonceThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o.", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void NullOrEmptyNonceThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void InvalidNonceFormatThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patr�n", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedThrows()
        {
            ServiceLocator.Instance.RegisterNonceGenerator(new SingleUseNonceGenerator());
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(DelegatedAppIdentity.Default)
                .GetClient();

            // Se puede autenticar la aplicaci�n usando el nonce la primera vez.
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);

            // No se podr� autentica la aplicaci�n, cuando use el mismo nonce por segunda vez.
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicaci�n", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void MissingEpochThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void NullOrEmptyEpochThrows()
        {
            IList<IPayloadClaimsManager> payloadClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => null),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadClaimBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void InvalidEpochFormatThrows()
        {
            IList<IPayloadClaimsManager> payloadClaimBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "x"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
            };

            foreach (IPayloadClaimsManager behavior in payloadClaimBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un n�mero.", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochExpiredThrows()
        {
            int randomDays = new Random().Next(2, 10);
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochExceededThrows()
        {
            int randomDays = new Random().Next(5, 10);
            AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochNegativeThrows()
        {
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un n�mero.", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.Epoch")]
        public void EpochZeroThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.DocType")]
        public void MissingUserDocTypeThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDocTypePayloadClaim.AvoidingClaim());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocType' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.DocType")]
        public void NullOrEmptyUserDocTypeThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => null),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.DocType")]
        public void UnsupportedUserDocTypeThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "01"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "10"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "xx"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "XX"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "Xx"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "xX"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "cc"),
                InvalidDocTypePayloadClaim.WithClaimBehavior(() => "Cc")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no se reconoce como un tipo de identificaci�n", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.DocNumber")]
        public void MissingUserDocNumberThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDocNumberPayloadClaim.AvoidingClaim());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Headers.Payload.DocNumber")]
        public void NullOrEmptyUserDocNumberThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => null),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.DocNumber")]
        public void InvalidUserDocNumberFormatThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "a"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "abcdef"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "A0b1C2d3e4f5g6H7"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "$123456@123#456*"),
                InvalidDocNumberPayloadClaim.WithClaimBehavior(() => "97165682970991858533737047")
            };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch(@"'DocNumber' debe coincidir con el patr�n", exception.Message);
            }
        }

        [Test]
        [Category("Headers.Payload.DeviceId")]
        public void MissingDeviceIdThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidDeviceIdPayloadClaim.AvoidingClaim());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Default)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .AuthenticateNoCache(DelegatedAppIdentity.Default)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DeviceId' no puede ser nulo ni vac�o", exception.Message);
        }

        /// <summary>
        /// Autenticar una aplicaci�n sin la cabecera de la versi�n del API solicitada funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingApiVersionHeaderWorks()
        {
            ServiceLocator.Instance.RegisterHeadersManager(InvalidApiVersionHeader.AvoidingHeader());
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Default)
                .WithIdentity(DelegatedAppIdentity.Default)
                .AuthenticateNoCache(DelegatedAppIdentity.Default)
                .GetClient();

            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores nulos o vac�os en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullEmptyApiVersionHeaderThrows()
        {
            IList<IHeadersManager> apiVersionHeaderBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => null),
                InvalidApiVersionHeader.WithHeaderBehavior(() => string.Empty),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager headerBehavior in apiVersionHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
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
        [Category("Delegated.Signin.Headers")]
        public void InvalidApiVersionHeaderFormatThrows()
        {
            IList<IHeadersManager> apiVersionHeaderBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => "abc"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => Guid.NewGuid().ToString()),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "123"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "1,0"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "1A"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "-1.0")
            };

            foreach (IHeadersManager headerBehavior in apiVersionHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
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
        [Category("Delegated.Signin.Headers")]
        public void UnsupportedApiVersionHeaderThrows()
        {
            IList<IHeadersManager> apiVersionHeaderBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => "0.1"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999.999999"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999.999999.999999")
            };

            foreach (IHeadersManager headerBehavior in apiVersionHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Default)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .AuthenticateNoCache(DelegatedAppIdentity.Default)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99005"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un valor admitido para el encabezado personalizado 'X-PRO-Api-Version'", exception.Message);
            }
        }
    }
}