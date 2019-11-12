// -----------------------------------------------------------------------
// <copyright file="AutonomousAppTests.SigninHeaders.cs" company="Evertec Colombia">
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
    public partial class AutonomousAppTests
    {
        /// <summary>
        /// Autenticar una aplicaci�n cuando falta el encabezado del ApiKey no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiKey")]
        public void MissingApiKeyHeaderWhenAppSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterHeadersManager(InvalidApiKeyHeader.AvoidingHeader());
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
        }

        /// <summary>
        /// Autenticar una aplicaci�n cuando el encabezado del ApiKey es nulo o vac�o no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiKey")]
        public void NullOrEmptyApiKeyHeaderWhenAppSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiKeyHeader.WithHeaderBehavior(() => null),
                InvalidApiKeyHeader.WithHeaderBehavior(() => string.Empty),
                InvalidApiKeyHeader.WithHeaderBehavior(() => "      ")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-App'", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticaci�n sin el encabezado de la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload")]
        public void MissingPayloadHeaderWhenAppSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterHeadersManager(InvalidPayloadHeader.AvoidingHeader());
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("20002"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n con el encabezado de la carga �til nula o vac�a no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload")]
        public void NullOrEmptyPayloadHeaderWhenAppSigninRequestThrows()
        {
            IList<IHeadersManager> payloadHeaderBehaviors = new List<IHeadersManager>()
            {
                InvalidPayloadHeader.WithHeaderBehavior(() => null),
                InvalidPayloadHeader.WithHeaderBehavior(() => string.Empty),
                InvalidPayloadHeader.WithHeaderBehavior(() => "     ")
            };

            foreach (IHeadersManager headerBehavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(headerBehavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("20002"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Se requiere la cabecera personalizada 'X-PRO-Auth-Payload'", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticaci�n con una carga �til inv�lida no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload")]
        public void InvalidPayloadSignatureWhenAppSigninRequestThrows()
        {
            IHeadersManager invalidSignatureBehavior = InvalidPayloadHeader.WithHeaderBehavior(() => "Lorem ipsum dolor sit amet, consetetur sadipscing elitr");
            ServiceLocator.Instance.RegisterHeadersManager(invalidSignatureBehavior);
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("20007"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("El contenido de la cabecera personalizada 'X-PRO-Auth-Payload' no es v�lido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n sin nonce en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void MissingNonceWhenAppSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidNoncePayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n usando un nonce nulo o vac�o en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void NullOrEmptyNonceWhenAppSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>()
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => null),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => string.Empty),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "     ")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticaci�n usando un nonce con formato inv�lido en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void InvalidFormatNonceWhenAppSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadBehaviors = new List<IPayloadClaimsManager>
                                                                {
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => "gXjyhrYqannHUA$LLV&7guTHmF&1X5JB$Uobx3@!rPn9&x4BzE"),
                                                                    InvalidNoncePayloadClaim.WithClaimBehavior(() => $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}")
                                                                };

            foreach (IPayloadClaimsManager behavior in payloadBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Nonce' debe coincidir con el patr�n", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticaci�n usando un nonce que ya fue procesado no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Nonce")]
        public void NonceAlreadyProcessedWhenAppSigninRequestThrows()
        {
            SingleUseNonceGenerator singleUseNonceGenerator = new SingleUseNonceGenerator();
            ServiceLocator.Instance.RegisterNonceGenerator(singleUseNonceGenerator);

            // Se puede autenticar la aplicaci�n usando el nonce la primera vez.
            IAutonomousApp client = AuthenticateNoCache();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);

            // No se podr� autenticar la aplicaci�n, cuando use el mismo nonce por segunda vez.
            Assert.AreEqual(ServiceLocator.Instance.NonceGenerator.GetNonce(), singleUseNonceGenerator.GetNonce());
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Nonce ya procesado para su aplicaci�n", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n cuando falta el epoch en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void MissingEpochWhenAppSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterPayloadClaimsManager(InvalidEpochPayloadClaim.AvoidingClaim());
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15852"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n cuando el epoch es nulo o vac�o en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void NullOrEmptyEpochWhenAppSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => null),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => string.Empty),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "     ")
            };

            foreach (IPayloadClaimsManager behavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("15852"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("'Epoch' no puede ser nulo ni vac�o", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticaci�n cuando se establece un epoch con formato inv�lido en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void InvalidFormatEpochWhenAppSigninRequestThrows()
        {
            IList<IPayloadClaimsManager> payloadHeaderBehaviors = new List<IPayloadClaimsManager>()
            {
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "x"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "abcdef"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "a123b"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => "A123B"),
                InvalidEpochPayloadClaim.WithClaimBehavior(() => Guid.NewGuid().ToString())
            };

            foreach (IPayloadClaimsManager behavior in payloadHeaderBehaviors)
            {
                ServiceLocator.Instance.RegisterPayloadClaimsManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("15850"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un n�mero.", exception.Message);
            }
        }

        /// <summary>
        /// Una solicitud de autenticaci�n con un epoch que ya expir� en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochExpiredWhenAppSigninRequestThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(-randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n con un epoch muy adelante en el futuro en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochExceededWhenAppSigninRequestThrows()
        {
            int randomDays = new Random().Next(2, 10);
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromDatePicker(randomDays));
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n con un epoch negativo en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochNegativeWhenAppSigninRequestThrows()
        {
            double negativeSeconds = -DateTimeOffset.Now.ToUnixTimeSeconds();
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(negativeSeconds));
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15850"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            StringAssert.IsMatch("Formato de Epoch no es valido. Debe ser un n�mero.", exception.Message);
        }

        /// <summary>
        /// Una solicitud de autenticaci�n con un epoch igual a cero en la carga �til no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.Payload.Epoch")]
        public void EpochZeroWhenAppSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterEpochGenerator(FixedEpochGenerator.FromStaticSeconds(0));
            AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
            Assert.That(exception.EventId, Is.EqualTo("15851"));
            Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.RequestedRangeNotSatisfiable));
            StringAssert.IsMatch("Epoch est� fuera de rango admitido", exception.Message);
        }

        /// <summary>
        /// Autenticar una aplicaci�n debe funcionar cuando no se usa la cabecera de la versi�n del API en la solicitud.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void MissingApiVersionHeaderWhenAppSigninRequestThrows()
        {
            ServiceLocator.Instance.RegisterHeadersManager(InvalidApiVersionHeader.AvoidingHeader());
            IAutonomousApp client = AuthenticateNoCache();
            Assert.That(client, Is.Not.Null);
            Assert.That(client.AuthToken, Is.Not.Null);
            Assert.That(client.AuthToken.Token, Is.Not.Null);
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores nulos o vac�os en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void NullOrEmptyApiVersionHeaderWhenAppSigninRequestThrows()
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
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato v�lido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores inv�lidos en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void InvalidFormatApiVersionHeaderWhenAppSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => "abc"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => Guid.NewGuid().ToString()),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "123"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "1,0"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "1A"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "-1.0")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("99001"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un formato v�lido para el encabezado 'X-PRO-Api-Version'", exception.Message);
            }
        }

        /// <summary>
        /// Autenticar una aplicaci�n con valores no soportados en la cabecera de la versi�n del API solicitada no funciona.
        /// </summary>
        [Test]
        [Category("Signin.Headers.ApiVersion")]
        public void UnsupportedApiVersionHeaderWhenAppSigninRequestThrows()
        {
            IList<IHeadersManager> headerBehaviors = new List<IHeadersManager>()
            {
                InvalidApiVersionHeader.WithHeaderBehavior(() => "0.1"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999.999999"),
                InvalidApiVersionHeader.WithHeaderBehavior(() => "999999.999999.999999.999999")
            };

            foreach (IHeadersManager behavior in headerBehaviors)
            {
                ServiceLocator.Instance.RegisterHeadersManager(behavior);
                AspenException exception = Assert.Throws<AspenException>(() => AuthenticateNoCache());
                Assert.That(exception.EventId, Is.EqualTo("99005"));
                Assert.That(exception.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                StringAssert.IsMatch("no es un valor admitido para el encabezado personalizado 'X-PRO-Api-Version'", exception.Message);
            }
        }
    }
}