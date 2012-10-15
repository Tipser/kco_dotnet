﻿#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="IConnector.cs" company="Klarna AB">
//     Copyright 2012 Klarna AB
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//         http://www.apache.org/licenses/LICENSE-2.0
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
// <author>Klarna Support: support@klarna.com</author>
// <link>http://integration.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Checkout
{
    /// <summary>
    /// The Connector interface.
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// Applies a HTTP method on a specific resource.
        /// </summary>
        /// <param name="method">
        /// The HTTP method.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        void Apply(HttpMethod method, IResource resource);
    }
}