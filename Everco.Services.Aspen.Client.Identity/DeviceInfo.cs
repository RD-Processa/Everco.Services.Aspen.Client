﻿// -----------------------------------------------------------------------
// <copyright file="DeviceInfo.cs" company="Evertec Colombia">
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>atorrest</author>
// <date>2019-11-29 01:02 PM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Identity
{
    using System;
    using System.Management;
    using System.Runtime.InteropServices;
    using Newtonsoft.Json;

    /// <summary>
    /// Representa la información del dispositivo que envía la petición al servicio Aspen.
    /// </summary>
    /// <seealso cref="IDeviceInfo" />
    public class DeviceInfo : IDeviceInfo
    {
        /// <summary>
        /// Para uso interno.
        /// </summary>
        private const string Unspecified = "Unspecified";

        /// <summary>
        /// Para uso interno.
        /// </summary>
        private string currentJson;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="DeviceInfo"/>.
        /// </summary>
        private DeviceInfo()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                throw new PlatformNotSupportedException();
            }

            try
            {
                SelectQuery query = new SelectQuery(@"Select * from Win32_ComputerSystem");
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    foreach (ManagementBaseObject process in searcher.Get())
                    {
                        this.Manufacturer = process["Manufacturer"].ToString();
                        this.Model = process["Model"].ToString();
                    }
                }
            }
            catch
            {
                this.Manufacturer = Unspecified;
                this.Model = Unspecified;
            }

            this.DeviceId = $@"{Environment.UserDomainName}\{Environment.UserName}";
            this.Name = Environment.MachineName;
            this.Version = Environment.OSVersion.VersionString;
            this.Platform = Environment.OSVersion.Platform.ToString();
            this.Idiom = "Desktop";
            this.DeviceType = "Physical";
        }

        /// <summary>
        /// Obtiene una instancia con la información predeterminada de un dispositivo.
        /// </summary>
        public static IDeviceInfo Current => new DeviceInfo();

        /// <summary>
        /// Obtiene o establece el identificador del dispositivo.
        /// </summary>
        /// <remarks>
        /// Una cadena que identifique de manera unívoca al dispositivo del usuario.
        /// Valor predeterminado: <c>$@"{Environment.UserDomainName}\{Environment.UserName}"</c>
        /// </remarks>
        public string DeviceId { get; set; }

        /// <summary>
        /// Obtiene o establece una cadena que identifica si el dispositivo es físico o virtual.
        /// </summary>
        /// <example>Virtual(cuando se trata de un Emulador) o Physical.</example>
        public string DeviceType { get; set; }

        /// <summary>
        /// Obtiene o establece el "tipo" de dispositivo.
        /// </summary>
        /// <example>Tablet,Phone,TV,Desktop,CarPlay,Unspecified, etc.</example>
        public string Idiom { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del fabricante del dispositivo.
        /// </summary>
        /// <example>Samsung, Apple, LG, Motorola, etc</example>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Obtiene o establece el modelo del dispositivo.
        /// </summary>
        /// <example>SMG-950U, iPhone10,6, iPhone10,1, iPhone5,3, iPad6,3</example>
        public string Model { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre asociado con el dispositivo.
        /// </summary>
        /// <remarks>
        /// Para iOS vea: <a href="https://support.apple.com/en-us/HT201997">Name of your iPhone</a>.
        /// Para Android vea: <a href="https://www.wikihow.tech/Change-the-Name-of-Your-Android-Phone">Name of Your Android Phone</a>.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Obtiene o establece el nombre del sistema operativo del dispositivo.
        /// </summary>
        /// <example>Android, iOS</example>
        public string Platform { get; set; }

        /// <summary>
        /// Obtiene o establece la versión del sistema operativo del dispositivo.
        /// </summary>        
        public string Version { get; set; }

        /// <summary>
        /// Obtiene la representación de la instancia actual en formato Json.
        /// </summary>
        /// <returns>Cadena en formato Json que representa los valores de la instancia.</returns>
        public string ToJson()
        {
            if (!string.IsNullOrWhiteSpace(this.currentJson))
            {
                return this.currentJson;
            }

            this.currentJson = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None);
            return this.currentJson;
        }
    }
}