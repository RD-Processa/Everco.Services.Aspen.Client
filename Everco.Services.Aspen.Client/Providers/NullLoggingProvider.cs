﻿// -----------------------------------------------------------------------
// <copyright file="NullLoggingProvider.cs" company="Evertec Colombia"> 
// Copyright (c) 2019 Todos los derechos reservados.
// </copyright>
// <author>dmontalvo</author>
// <date>2019-09-26 10:17 AM</date>
// ----------------------------------------------------------------------
namespace Everco.Services.Aspen.Client.Providers
{
    using System;

    /// <summary>
    /// Implementa un tipo que puede omite la escritura de información de seguimiento.
    /// </summary>
    /// <seealso cref="ILoggingProvider" />
    public class NullLoggingProvider : ILoggingProvider
    {
        /// <summary>
        /// Omite escribir información de depuración.
        /// </summary>
        /// <param name="message">Texto del mensaje de depuración.</param>
        public void WriteDebug(string message)
        {
        }

        /// <summary>
        /// mite escribir información de un objeto.
        /// </summary>
        /// <param name="message">Texto del mensaje de información.</param>
        /// <param name="object">Instancia del objeto a escribir.</param>
        public void WriteInfo(string message, object @object = null)
        {
        }

        /// <summary>
        /// mite escribir información de un error.
        /// </summary>
        /// <param name="exception">Excepción que ocasion+o el error.</param>
        /// <param name="message">Texto del mensaje de error.</param>
        public void WriteError(Exception exception, string message = null)
        {
        }
    }
}