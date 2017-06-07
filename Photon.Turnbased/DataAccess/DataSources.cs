using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Photon.Webhooks.Turnbased.Config;

namespace Photon.Webhooks.Turnbased.DataAccess
{
    public class DataSources
    {
        public IDataAccess DataAccess { get; private set; }
        private readonly AppSettings _appSettings;
        private readonly ConnectionStrings _connectionStrings;

        public DataSources(IOptions<AppSettings> dataAccessor, IOptions<ConnectionStrings> connectionStrings, ILogger<Azure> logger)
        {
            _appSettings = dataAccessor.Value;
            _connectionStrings = connectionStrings.Value;
            CreatDataAccessor(logger);
        }

        private void CreatDataAccessor(ILogger<Azure> logger)
        {
            if (_appSettings.DataSource.Equals("Azure", StringComparison.OrdinalIgnoreCase))
            {
                DataAccess = new Azure(_connectionStrings.AzureBlobConnectionString, logger);
            }
            else if (_appSettings.DataSource.Equals("Redis", StringComparison.OrdinalIgnoreCase))
            {
                //TODO: Setup up the redis local cache here   
            }
        }
        
    }
}
