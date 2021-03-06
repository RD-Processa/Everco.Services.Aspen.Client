﻿// -----------------------------------------------------------------------
// <copyright file="IdentityTests.cs" company="Processa">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-27 02:27 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests.Identities
{
    using System;
    using System.Text;
    using Identity;
    using NUnit.Framework;

    [TestFixture]
    public class IdentityTests
    {
        [Test]
        [Category("AppIdentities")]
        public void RegistryIdentityWorks()
        {
            RegistryIdentity identity = new RegistryIdentity(
                RegistryRoot.LocalMachine,
                @"SOFTWARE\Aspen\Credentials",
                "ApiKey",
                "ApiSecret");

            Assert.That(identity.ApiKey, Is.Not.Null);
            Assert.That(identity.ApiSecret, Is.Not.Null);
            
            Console.WriteLine(identity.ApiKey);
            Console.WriteLine(identity.ApiSecret);
        }

        [Test]
        [Category("AppIdentities")]
        public void SecureIdentityWorks()
        {
            // Primer paso (Guardar en un archivo, una única vez)
            string apiKey = Guid.NewGuid().ToString("D");
            string apiSecret = TestContext.CurrentContext.Random.GetString(150);
            SecureFileIdentity fileIdentity = new SecureFileIdentity(apiKey, apiSecret);
            string fileName = $@"D:\Temp\{Guid.NewGuid():N}.bin";
            fileIdentity.Encrypt().SaveTo(fileName);

            // Segundo paso (Leer del archivo guardado, tantas veces como se necesite)
            SecureFileIdentity fileIdentityFromFile = SecureFileIdentity.FromFile(fileName);
            Assert.That(fileIdentityFromFile.ApiKey, Is.EqualTo(apiKey));
            Assert.That(fileIdentityFromFile.ApiSecret, Is.EqualTo(apiSecret));
        }

        [Test]
        [Category("AppIdentities")]
        public void SecureIdentityWithEntropyWorks()
        {
            // Primer paso (Guardar en un archivo, una única vez, utilizando un valor para la entropia)
            string apiKey = Guid.NewGuid().ToString("D");
            string apiSecret = TestContext.CurrentContext.Random.GetString(150);
            string entropyText = TestContext.CurrentContext.Random.GetString(100);
            EncodingInfo[] encodings = Encoding.GetEncodings();
            int ordinal = TestContext.CurrentContext.Random.Next(encodings.Length);
            Encoding encoding = Encoding.GetEncoding(encodings[ordinal].CodePage);
            byte[] entropy = encoding.GetBytes(entropyText);
            SecureFileIdentity fileIdentity = new SecureFileIdentity(apiKey, apiSecret, entropy, encoding);
            string fileName = $@"D:\Temp\{Guid.NewGuid():N}.bin";
            fileIdentity.Encrypt().SaveTo(fileName);

            // Segundo paso (Leer del archivo guardado, tantas veces como se necesite)
            SecureFileIdentity fileIdentityFromFile = SecureFileIdentity.FromFile(fileName);
            Assert.That(fileIdentityFromFile.ApiKey, Is.EqualTo(apiKey));
            Assert.That(fileIdentityFromFile.ApiSecret, Is.EqualTo(apiSecret));
        }

        [Test]
        [Category("AppIdentities")]
        public void EnvironmentIdentityWorks()
        {
            EnvironmentIdentity identity = new EnvironmentIdentity(
                "ASPEN:AUTONOMOUSAPP2:APIKEY",
                "ASPEN:AUTONOMOUSAPP2:APISECRET");

            Assert.IsNotNull(identity.ApiKey);
            Assert.IsNotNull(identity.ApiSecret);
        }
    }
}