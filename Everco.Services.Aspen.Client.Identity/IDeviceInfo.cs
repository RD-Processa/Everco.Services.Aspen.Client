﻿// -----------------------------------------------------------------------
// <copyright file="IDeviceInfo.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-29 12:48 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    /// <summary>
    /// Define la información asociada con el dispositivo de un usuario que envía la petición al servicio Aspen.
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// Obtiene o establece el identificador del dispositivo.
        /// </summary>
        /// <remarks>
        /// Una cadena que identifique de manera unívoca al dispositivo del usuario.
        /// </remarks>
        string DeviceId { get; set; }

        /// <summary>
        /// Obtiene o establece una cadena que identifica si el dispositivo es físico o virtual.
        /// </summary>
        /// <example>Virtual(cuando se trata de un Emulador) o Physical.</example>
        string DeviceType { get; set; }

        /// <summary>
        /// Obtiene o establece el "tipo" de dispositivo.
        /// </summary>
        /// <example>Tablet,Phone,TV,Desktop,CarPlay,Unspecified, etc.</example>
        string Idiom { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del fabricante del dispositivo.
        /// </summary>
        /// <example>Samsung, Apple, LG, Motorola, etc</example>
        string Manufacturer { get; set; }

        /// <summary>
        /// Obtiene o establece el modelo del dispositivo.
        /// </summary>
        /// <example>SMG-950U, iPhone10,6, iPhone10,1, iPhone5,3, iPad6,3</example>
        string Model { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre asociado con el dispositivo.
        /// </summary>
        /// <remarks>
        /// Para iOS vea: <a href="https://support.apple.com/en-us/HT201997">Name of your iPhone</a>.
        /// Para Android vea: <a href="https://www.wikihow.tech/Change-the-Name-of-Your-Android-Phone">Name of Your Android Phone</a>.
        /// </remarks>
        string Name { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del sistema operativo del dispositivo.
        /// </summary>
        /// <example>Android, iOS</example>
        string Platform { get; set; }

        /// <summary>
        /// Obtiene o establece la versión del sistema operativo del dispositivo.
        /// </summary>        
        string Version { get; set; }

        /// <summary>
        /// Obtiene la representación de la instancia actual en formato Json.
        /// </summary>
        /// <returns>Cadena en formato Json que representa los valores de la instancia.</returns>
        string ToJson();
    }
}