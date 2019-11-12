// -----------------------------------------------------------------------
// <copyright file="DelegatedAppTests.Settings.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-24 10:00 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Tests
{
    using System.Collections.Generic;
    using Everco.Services.Aspen.Entities;
    using NUnit.Framework;

    /// <summary>
    /// Implementa las pruebas unitarias para acceder a las entidades de informaci�n relacionadas con parametrizaci�n del sistema para una aplicaci�n de alcance de delegada.
    /// </summary>
    [TestFixture]
    public partial class DelegatedAppTests
    {
        /// <summary>
        /// Obtener los tipos de documento de la aplicaci�n funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetDocTypesWorks()
        {
            IList<DocTypeInfo> docTypes = Client.Settings.GetDocTypes();
            CollectionAssert.IsNotEmpty(docTypes);
        }

        /// <summary>
        /// Obtener los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicaci�n funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetPaymentTypesWorks()
        {
            IList<PaymentTypeInfo> paymentTypes = Client.Settings.GetPaymentTypes();
            CollectionAssert.IsNotEmpty(paymentTypes);
        }

        /// <summary>
        /// Obtiener los valores admitidos de recarga por operador para la aplicaci�n solicitante funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetTopUpValuesWorks()
        {
            IList<TopUpInfo> topUpValues = Client.Settings.GetTopUpValues();
            CollectionAssert.IsNotEmpty(topUpValues);
        }

        /// <summary>
        /// Obtener la lista de los tipos de transacci�n soportados para la aplicaci�n solicitante funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetTranTypesWorks()
        {
            IList<TranTypeInfo> tranTypes = Client.Settings.GetTranTypes();
            CollectionAssert.IsNotEmpty(tranTypes);
        }

        /// <summary>
        /// Obtener la lista de opciones que representan el men� para la aplicaci�n m�vil funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetAppMenuItemsWorks()
        {
            IList<MenuItemInfo> menuItems = Client.Settings.GetMenu();
            CollectionAssert.IsNotEmpty(menuItems);
        }

        /// <summary>
        /// Obtener la configuraci�n de valores miscel�neos soportados para la aplicaci�n funciona.
        /// </summary>
        [Test]
        [Category("Modules.Settings")]
        public void GetMiscellaneousSettingsWorks()
        {
            MiscellaneousSettings miscellaneous = Client.Settings.GetMiscellaneousSettings();
            CollectionAssert.IsNotEmpty(miscellaneous);
        }
    }
}