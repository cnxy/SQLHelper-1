﻿/*
Copyright 2016 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using Microsoft.Extensions.Configuration;
using SQLHelper.HelperClasses.Interfaces;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace SQLHelper.HelperClasses
{
    /// <summary>
    /// Data source class
    /// </summary>
    /// <seealso cref="IConnection"/>
    public class Connection : IConnection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Connection"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="name">The name.</param>
        public Connection(IConfiguration configuration, DbProviderFactory factory, string name)
            : this(configuration, factory, "", name)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="name">The name.</param>
        /// <param name="parameterPrefix">The parameter prefix.</param>
        /// <exception cref="System.ArgumentNullException">configuration</exception>
        public Connection(IConfiguration configuration, DbProviderFactory factory, string connection, string name, string parameterPrefix = "@")
        {
            Configuration = configuration ?? throw new System.ArgumentNullException(nameof(configuration));
            Name = string.IsNullOrEmpty(name) ? "Default" : name;
            SourceType = factory.GetType().FullName;
            Factory = factory;
            var TempConfig = configuration.GetConnectionString(Name);
            if (string.IsNullOrEmpty(connection) && TempConfig != null)
            {
                ConnectionString = TempConfig;
            }
            else
            {
                ConnectionString = string.IsNullOrEmpty(connection) ? name : connection;
            }
            if (string.IsNullOrEmpty(parameterPrefix))
            {
                if (SourceType.Contains("MySql"))
                    ParameterPrefix = "?";
                else if (SourceType.Contains("Oracle"))
                    ParameterPrefix = ":";
                else
                {
                    DatabaseName = Regex.Match(ConnectionString, @"Initial Catalog=([^;]*)").Groups[1].Value;
                    ParameterPrefix = "@";
                }
            }
            else
            {
                ParameterPrefix = parameterPrefix;
                if (SourceType.Contains("SqlClient"))
                {
                    DatabaseName = Regex.Match(ConnectionString, @"Initial Catalog=([^;]*)").Groups[1].Value;
                }
            }
        }

        /// <summary>
        /// Gets the configuration information.
        /// </summary>
        /// <value>Gets the configuration information.</value>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <value>The database.</value>
        public string DatabaseName { get; protected set; }

        /// <summary>
        /// Gets the factory that the system uses to actually do the connection.
        /// </summary>
        /// <value>The factory that the system needs to actually do the connection.</value>
        public DbProviderFactory Factory { get; protected set; }

        /// <summary>
        /// Name of the source
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; protected set; }

        /// <summary>
        /// Parameter prefix that the source uses
        /// </summary>
        /// <value>The parameter prefix.</value>
        public string ParameterPrefix { get; protected set; }

        /// <summary>
        /// Source type, based on ADO.Net provider name or identifier used by CUL
        /// </summary>
        /// <value>The type of the source.</value>
        public string SourceType { get; protected set; }
    }
}