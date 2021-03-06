﻿#region Copyright Header
// ----------------------------------------------------------------------------
// <copyright file="RecurringCreate.cs" company="Klarna AB">
//     Copyright 2014 Klarna AB
//
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//
//         http://www.apache.org/licenses/LICENSE-2.0
//
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.
// </copyright>
// <author>Klarna Support: support@klarna.com</author>
// <link>http://developers.klarna.com/</link>
// ----------------------------------------------------------------------------
#endregion
namespace Klarna.Kco.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Text;
    using Klarna.Checkout;

    /// <summary>
    /// The create recurring order example.
    /// </summary>
    public class RecurringCreate
    {
        /// <summary>
        /// The example.
        /// </summary>
        public static void Main()
        {
            /*
             Note! First you must've created a regular aggregated order with the option "recurring" set to true.
             After that order has recieved either status "checkout_complete" or "created" you can fetch that
             resource and retrieve the "recurring_token" property which is needed to create recurring orders.
             */

            const string Eid = "0";
            const string SharedSecret = "sharedSecret";
            const string RecurringToken = "ABC-123";

            var connector = Connector.Create(SharedSecret, Connector.TestBaseUri);

            RecurringOrder recurringOrder = new RecurringOrder(connector, RecurringToken);

            // Set optional merchant reference ids. Like internal order or customer id
            var merchant_reference = new Dictionary<string, object>
                {
                    { "orderid1", "123456789" },
                    { "orderid2", "987654321" }
                };

            var merchant = new Dictionary<string, object>
                {
                    { "id", Eid }
                };

            var items = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                    {
                        { "reference", "123456789" },
                        { "name", "Klarna t-shirt" },
                        { "quantity", 2 },
                        { "unit_price", 12300 },
                        { "discount_rate", 1000 },
                        { "tax_rate", 2500 }
                    },
                new Dictionary<string, object>
                    {
                        { "type", "shipping_fee" },
                        { "reference", "SHIPPING" },
                        { "name", "Shipping Fee" },
                        { "quantity", 1 },
                        { "unit_price", 4900 },
                        { "tax_rate", 2500 }
                    }
            };

            var cart = new Dictionary<string, object> { { "items", items } };

            var address = new Dictionary<string, object>
                {
                    { "given_name", "Testperson-se" },
                    { "family_name", "Approved" },
                    { "street_address", "Stårgatan 1" },
                    { "postal_code", "12345" },
                    { "city", "Ankeborg" },
                    { "country", "se" },
                    { "email", "checkout-se@testdrive.klarna.com" },
                    { "phone", "070 111 11 11" }
                };

            // If the order should be activated automatically.
            // Set to true if you instead want a invoice created
            // otherwise you will get a reservation.
            var activate = false;

            var data = new Dictionary<string, object>
                {
                    { "purchase_country", "SE" },
                    { "purchase_currency", "SEK" },
                    { "locale", "sv-se" },
                    { "merchant", merchant },
                    { "merchant_reference", merchant_reference },
                    { "cart", cart },
                    { "activate", activate },
                    { "billing_address", address },
                    { "shipping_address", address }
                };

            try
            {
                recurringOrder.Create(data);

                var result = recurringOrder.Marshal();
                string number = null;

                // If the recurring order wasn't activated we should have a reservation number
                if (result.ContainsKey("reservation"))
                {
                    number = (string)result["reservation"];
                }

                // If the recurring order was activated we should have an invoice number
                if (result.ContainsKey("invoice"))
                {
                    number = (string)result["invoice"];
                }

                Debug.Print(number);
            }
            catch (ConnectorException ex)
            {
                var webException = ex.InnerException as WebException;
                if (webException != null)
                {
                    // Here you can check for timeouts, and other connection related errors.
                    // webException.Response could contain the response object.
                }
                else
                {
                    // In case there wasn't a WebException where you could get the response
                    // (e.g. a protocol error, bad digest, etc) you might still be able to
                    // get a hold of the response object.
                    // ex.Data["Response"] as IHttpResponse
                }

                // Additional data might be available in ex.Data.
                if (ex.Data.Contains("internal_message"))
                {
                    // For instance, Content-Type application/vnd.klarna.error-v1+json has "internal_message".
                    var internalMessage = (string)ex.Data["internal_message"];
                    Debug.WriteLine(internalMessage);
                }

                if (ex.Data.Contains("reason"))
                {
                    // For instance, Content-Type application/vnd.klarna.checkout.recurring-order-rejected-v1+json
                    // has "reason".
                    var reason = (string)ex.Data["reason"];
                    Debug.WriteLine(reason);
                }

                throw;
            }
            catch (Exception)
            {
                // Something else went wrong, e.g. invalid arguments passed to the order object.
                throw;
            }
        }
    }
}
