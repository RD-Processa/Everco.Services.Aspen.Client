// -----------------------------------------------------------------------
// <copyright file="CacheKeys.cs" company="Evertec Colombia">
// Copyright (c) 2020 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2020-06-09 09:13 AM</date>
// -----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Internals
{
    /// <summary>
    /// Define los identificadores o claves para las entradas administradas por cach�.
    /// </summary>
    internal class CacheKeys
    {
        /// <summary>
        /// Identificador de la entrada en el cach� de la informaci�n de configuraci�n soportada por una aplicaci�n.
        /// </summary>
        internal const string AppMovSettings = "APP_MOV_SETTINGS";

        /// <summary>
        /// Identificador de la entrada en el cach� de la lista de operadores de telefon�a m�vil soportados para la aplicaci�n.
        /// </summary>
        internal const string Carriers = "RESX_CARRIERS";

        /// <summary>
        /// Identificador de la entrada en el cach� de la informaci�n del token de autenticaci�n emitido.
        /// </summary>
        internal const string CurrentAuthToken = "CURRENT_AUTH_TOKEN";

        /// <summary>
        /// Identificador de la entrada en el cach� de la informaci�n asociada con el dispositivo actual del usuario.
        /// </summary>
        internal const string CurrentDevice = "CURRENT_DEVICE_INFO";

        /// <summary>
        /// Identificador de la entrada en el cach� de la lista de tipos de documento soportados para la aplicaci�n.
        /// </summary>
        internal const string DocTypes = "RESX_DOCUMENT_TYPES";

        /// <summary>
        /// Identificador de la entrada en el cach� de la lista de opciones que representan el men� de una aplicaci�n m�vil.
        /// </summary>
        internal const string MenuItems = "RESX_MENU_ITEMS";

        /// <summary>
        /// Identificador de la entrada en el cach� de la configuraci�n de valores miscel�neos soportados para la aplicaci�n.
        /// </summary>
        internal const string MiscellaneousSettings = "RESX_MISCELLANEOUS_SETTINGS";

        /// <summary>
        /// Identificador de la entrada en el cach� de los tipos de pagos que se pueden realizar a una cuenta soportados para la aplicaci�n.
        /// </summary>
        internal const string PaymentTypes = "RESX_PAYMENT_TYPES";

        /// <summary>
        /// Identificador de la entrada en el cach� de la lista de canales soportados para la generaci�n de tokens o claves transaccionales.
        /// </summary>
        internal const string TokenChannels = "TOKEN_CHANNELS";

        /// <summary>
        /// Identificador de la entrada en el cach� de los valores admitidos de recarga por operador soportados para la aplicaci�n.
        /// </summary>
        internal const string TopUpValues = "RESX_TOPUP_VALUES";

        /// <summary>
        /// Identificador de la entrada en el cach� de la lista de los tipos de transacci�n soportados para la aplicaci�n.
        /// </summary>
        internal const string TranTypes = "RESX_TRANSACTION_TYPES";
    }
}