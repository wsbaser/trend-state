{
  "cluster_setting": {
    "ProductKey": "9673-87A0-35DF-D887-6F1B-561F-F862-637E-C133-EEA6-194D-B930-290E-F33C-E5F6-51ED",
    "AuthMode": "Forms",
    "InstallDatabase": true,
    "InstallSSMS": true,
    "SearchServicePort": 8101,
    "ClientName": "DefaultClient",
    "KmDatabase": {
      "+RemoteUserName": "snhiroY3cDUr//wFw6tzJw==",
      "+RemoteUserPassword": "ye+K0V4fd8LzIse0TR6FUg==",
      "SearchServer": "{{server}}",
      "KmFlavor": "FULL",
      "DeliveryServer": "{{server}}",
      "DeliveryHostName": "{{server}}",
      "InitialUser": {
        "UserName": "kmadmin",
        "+Password": "6LyABpIKJueEWHCkEDGZ5w==",
        "FirstName": "Test",
        "MiddleName": "",
        "LastName": "Admin",
        "Email": "test.admin@westkm.com"
      },
      "Name": "km",
      "Server": "c273krmwestkm.tlr.thomson.com",
      "Port": 1433,
      "+User": "vVudue9xgfbMYF8biXbPfA==",
      "+Password": "vVudue9xgfbMYF8biXbPfA==",
      "+SaUserName": "XWGoUJa1v2XHENjm46qPyw==",
      "+SaUserPassword": "XAN+xWDsMhgvGwmGajDsIw==",
      "DataPath": "{{dataPath}}\\MsSql"
    },
    "KmRidDatabase": {
      "Name": "kmrid",
      "Server": "c273krmwestkm.tlr.thomson.com",
      "Port": 1433,
      "+User": "vVudue9xgfbMYF8biXbPfA==",
      "+Password": "vVudue9xgfbMYF8biXbPfA==",
      "+SaUserName": "XWGoUJa1v2XHENjm46qPyw==",
      "+SaUserPassword": "XAN+xWDsMhgvGwmGajDsIw==",
      "DataPath": "{{dataPath}}\\MsSql"
    },
    "Email": {
      "Server": "{{server}}",
      "Port": 25,
      "User": "",
      "+Password": ""
    },
    "KmUi": {
      "GridPageSize": 10,
      "WorkspacePath": "{{dataPath}}\\Workspace",
      "VirtualDirectory": "km",
      "ApplicationUrl": "http://c273krmwestkm.tlr.thomson.com/KM",
      "UseSsl": false
    },
    "Server": {
      "InstallDir": "C:\\Program Files\\Thomson"
    },
    "DataPath": "C:\\ProgramData\\Thomson",
    "Elasticsearch": {
      "Uri": "http://c273krmwestkm.tlr.thomson.com:9200/",
      "DataPath": "{{dataPath}}\\ElasticSearch",
      "ZenHosts": "\"c273krmwestkm.tlr.thomson.com\"",
      "Server": "c273krmwestkm.tlr.thomson.com",
      "ClusterName": "cluster-c273krmwestkm.tlr.thomson.com",
      "NodeName": "node-{{server}}",
      "DataPort": 9200,
      "JvmHeap": 2048
    },
    "Logging": {
      "Path": "{{dataPath}}\\Logs",
      "Level": "INFO",
      "DebugJson": false,
      "Size": "50MB",
      "Archives": 10,
      "MaxDaysOld": 7
    },
    "RemoteUser": {
      "+userName": "i7fbf8n8bLZ7CJBHRbw9PA==",
      "+domain": "x9LT+z7MxOE8kakQ2r6k/Q==",
      "+password": "CczkJ7M2fnm5jD98P75a4g=="
    }
  },
  "c273krmwestkm.tlr.thomson.com": {
    "Features": [
      {
        "FeatureName": "Es",
        "Components": [
          {
            "Name": "km-elasticsearch-service-install"
          },
          {
            "Name": "km-metricbeat-install"
          },
          {
            "Name": "km-logcleanup-install"
          },
          {
            "Name": "km-filebeat-install"
          },
          {
            "Name": "km-es-curator-install"
          }
        ]
      },
      {
        "FeatureName": "Ui",
        "Components": [
          {
            "Name": "km-search-service-install"
          },
          {
            "Name": "km-ui-install"
          },
          {
            "Name": "km-kibana-install"
          }
        ]
      },
      {
        "FeatureName": "Staging",
        "Components": [
          {
            "Name": "km-staging-service-install"
          }
        ]
      },
      {
        "FeatureName": "Indexing",
        "Components": [
          {
            "Name": "km-indexing-service-install"
          },
          {
            "Name": "km-cite-install"
          },
          {
            "Name": "km-indexing-litanalysis-install"
          },
          {
            "Name": "km-analyzer-engine-host-install"
          },
          {
            "Name": "km-indexing-jcare-install"
          }
        ]
      },
      {
        "FeatureName": "Db",
        "Components": [
          {
            "Name": "km-db-install"
          }
        ]
      }
    ],
    "Server": {
      "ServerName": "c273krmwestkm.tlr.thomson.com"
    }
  }
}
