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
    /// 
    /// </summary>
    [TestFixture]
    public class DelegatedSigninHeadersTests
    {
        [SetUp]
        public void Setup()
        {
            ServiceLocator.Instance.Reset();
            ServiceLocator.Instance.RegisterInstanceOfWebProxy(new WebProxy("http://192.168.2.70:8080", true));
            ServiceLocator.Instance.RegisterInstanceOfLoggingProvider(new ConsoleLoggingProvider());
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void SigninRequestWorks()
        {
            IDelegatedApp client = DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
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
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(AutonomousAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
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
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(randomApiKey, apiKeySecret)
                    .Authenticate(DelegatedAppIdentity.Default, false)
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
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(apiKey, randomApiSecret)
                    .Authenticate(DelegatedAppIdentity.Default, false)
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
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new MissingApiKeyHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15842"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyApiKeyHeaderThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new MissingApiKeyHeader(HeaderValueBehavior.Null),
                new MissingApiKeyHeader(HeaderValueBehavior.Empty),
                new MissingApiKeyHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new MissingApiKeyHeader(HeaderValueBehavior.Null));
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15842"));
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
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new MissingPayloadHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15845"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyPayloadHeaderThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingPayloadHeader(HeaderValueBehavior.Null),
                new MissingPayloadHeader(HeaderValueBehavior.Empty),
                new MissingPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15845"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidPayloadSignatureThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new MissingPayloadHeader(HeaderValueBehavior.UnexpectedFormat));
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingNonceThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new MissingNonceClaimOnPayloadHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o.", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyNonceThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingNonceClaimOnPayloadHeader(HeaderValueBehavior.Null),
                new MissingNonceClaimOnPayloadHeader(HeaderValueBehavior.Empty),
                new MissingNonceClaimOnPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidNonceFormatThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingNonceClaimOnPayloadHeader(HeaderValueBehavior.UnexpectedFormat),
                new MissingNonceClaimOnPayloadHeader(HeaderValueBehavior.MaxLengthExceeded)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patr�n", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NonceAlreadyProcessedThrows()
        {
            Guid duplicatedNonce = Guid.NewGuid();
            ServiceLocator.Instance.RegisterInstanceOfNonceGenerator(new DuplicatedNonceGenerator(duplicatedNonce));
            IDelegatedApp client = DelegatedApp.Initialize()
                .RoutingTo(EnvironmentEndpointProvider.Local)
                .WithIdentity(DelegatedAppIdentity.Default)
                .Authenticate(DelegatedAppIdentity.Default, false)
                .GetClient();

            // Se puede autenticar la aplicaci�n usando el nonce la primera vez.
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);

            // No se podr� autentica la aplicaci�n, cuando use el mismo nonce por segunda vez.
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicaci�n", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingEpochThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new MissingEpochClaimOnPayloadHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyEpochThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingEpochClaimOnPayloadHeader(HeaderValueBehavior.Null),
                new MissingEpochClaimOnPayloadHeader(HeaderValueBehavior.Empty),
                new MissingEpochClaimOnPayloadHeader(HeaderValueBehavior.WhiteSpaces)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidEpochFormatThrows()
        {
            IList<IHeadersManager> payloadBehaviors = new List<IHeadersManager>()
            {
                new MissingEpochClaimOnPayloadHeader(HeaderValueBehavior.UnexpectedFormat),
                new MissingEpochClaimOnPayloadHeader(HeaderValueBehavior.MinLengthRequired),
                new MissingEpochClaimOnPayloadHeader(HeaderValueBehavior.MaxLengthExceeded)
            };

            foreach (IHeadersManager behavior in payloadBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un n�mero.", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void EpochExpiredThrows()
        {
            IList<IEpochGenerator> epochBehaviors = new List<IEpochGenerator>()
            {
                new PastUnixEpochGenerator(),
                new FutureUnixEpochGenerator(),
            };

            foreach (IEpochGenerator behavior in epochBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfEpochGenerator(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15851"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
                StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingUserDocTypeThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new UnsupportedDocTypeOnPayloadHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocType' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyUserDocTypeThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedDocTypeOnPayloadHeader(null),
                new UnsupportedDocTypeOnPayloadHeader(string.Empty),
                new UnsupportedDocTypeOnPayloadHeader("  ")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocType' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void UnsupportedUserDocTypeThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedDocTypeOnPayloadHeader("01"),
                new UnsupportedDocTypeOnPayloadHeader("10"),
                new UnsupportedDocTypeOnPayloadHeader("xx"),
                new UnsupportedDocTypeOnPayloadHeader("XX"),
                new UnsupportedDocTypeOnPayloadHeader("Xx"),
                new UnsupportedDocTypeOnPayloadHeader("xX"),
                new UnsupportedDocTypeOnPayloadHeader("cc"),
                new UnsupportedDocTypeOnPayloadHeader("Cc")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no se reconoce como un tipo de identificaci�n", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingUserDocNumberThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new UnsupportedDocNumberOnPayloadHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyUserDocNumberThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedDocNumberOnPayloadHeader(null),
                new UnsupportedDocNumberOnPayloadHeader(string.Empty),
                new UnsupportedDocNumberOnPayloadHeader("  ")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidUserDocNumberFormatThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedDocNumberOnPayloadHeader("a"),
                new UnsupportedDocNumberOnPayloadHeader("abcdef"),
                new UnsupportedDocNumberOnPayloadHeader("A0b1C2d3e4f5g6H7"),
                new UnsupportedDocNumberOnPayloadHeader("$123456@123#456*"),
                new UnsupportedDocNumberOnPayloadHeader("97165682970991858533737047")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch(@"'DocNumber' debe coincidir con el patr�n", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void MissingUserDeviceIdThrows()
        {
            AspenException exception = Assert.Throws<AspenException>(() =>
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(new UnsupportedDocNumberOnPayloadHeader());
                DelegatedApp.Initialize()
                    .RoutingTo(EnvironmentEndpointProvider.Local)
                    .WithIdentity(DelegatedAppIdentity.Default)
                    .Authenticate(DelegatedAppIdentity.Default, false)
                    .GetClient();
            });

            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'DocNumber' no puede ser nulo ni vac�o", exception.Message);
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void NullOrEmptyApiVersionHeaderThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedApiVersionHeader(),
                new UnsupportedApiVersionHeader(string.Empty),
                new UnsupportedApiVersionHeader("  ")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato v�lido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void InvalidApiVersionHeaderFormatThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedApiVersionHeader("xxxx"),
                new UnsupportedApiVersionHeader("123"),
                new UnsupportedApiVersionHeader("1,0")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato v�lido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        [Test]
        [Category("Delegated.Signin.Headers")]
        public void UnsupportedApiVersionHeaderThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                new UnsupportedApiVersionHeader("0.1"),
                new UnsupportedApiVersionHeader("999999.999999"),
                new UnsupportedApiVersionHeader("999999.999999.999999"),
                new UnsupportedApiVersionHeader("999999.999999.999999.999999")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterInstanceOfApiSignManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() =>
                {
                    DelegatedApp.Initialize()
                        .RoutingTo(EnvironmentEndpointProvider.Local)
                        .WithIdentity(DelegatedAppIdentity.Default)
                        .Authenticate(DelegatedAppIdentity.Default, false)
                        .GetClient();
                });

                Assert.That(exception.EventId, Is.EqualTo("99005"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Valor no admitido para el encabezado personalizado 'X-PRO-Api-Version'", exception.Message);
            }
        }
    }
}