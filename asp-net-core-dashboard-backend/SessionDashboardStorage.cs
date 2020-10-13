using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace AspNetCoreDashboardCustomPropertiesSample {
    public class SessionDashboardStorage : DashboardStorageBase {
        const string DashboardStorageKey = "74cba564-c821-439c-a714-40ff6027b1eb";

        readonly IHttpContextAccessor сontextAccessor;

        protected HttpContext HttpContext { get { return сontextAccessor?.HttpContext; } }

        public SessionDashboardStorage(IHttpContextAccessor contextAccessor) : base() {
            сontextAccessor = contextAccessor;
        }

        private Dictionary<string, string> ReadFromSession() {
            Dictionary<string, string> result = null;
            ISession session = HttpContext?.Session;
            if(session != null) {
                string serializedStorage = session.GetString(DashboardStorageKey) ?? string.Empty;
                result = JsonConvert.DeserializeObject<Dictionary<string, string>>(serializedStorage);
                if(result == null) {
                    result = Initialize();
                    SaveToSession(result);
                }
            }
            return result;
        }
        private void SaveToSession(Dictionary<string, string> storage) {
            HttpContext?.Session?.SetString(DashboardStorageKey, JsonConvert.SerializeObject(storage));
        }
        private Dictionary<string, string> Initialize() {
            Dictionary<string, string> storage = new Dictionary<string, string>();
            foreach(string file in Directory.EnumerateFiles((string)AppDomain.CurrentDomain.GetData("DataDirectory"), "Dashboards/*.xml")) {
                InitializeCore(Path.GetFileNameWithoutExtension(file), storage);
            }
            return storage;
        }
        private void InitializeCore(string dashboardID, Dictionary<string, string> storage) {
            string dataDirectoryPath = (string)AppDomain.CurrentDomain.GetData("DataDirectory");
            string filePath = Path.Combine(dataDirectoryPath, "Dashboards", $"{dashboardID}.xml");
            if(!storage.ContainsKey(dashboardID) && File.Exists(filePath)) {
                using(StreamReader reader = new StreamReader(filePath)) {
                    storage.Add(dashboardID, reader.ReadToEnd());
                }
            }
        }

        protected override IEnumerable<string> GetAvailableDashboardsID() {
            return ReadFromSession().Keys;
        }
        protected override XDocument LoadDashboard(string dashboardID) {
            Dictionary<string, string> storage = ReadFromSession();
            return XDocument.Parse(storage[dashboardID]);
        }
        protected override void SaveDashboard(string dashboardID, XDocument dashboard, bool createNew) {
            Dictionary<string, string> storage = ReadFromSession();
            if(createNew ^ storage.ContainsKey(dashboardID)) {
                storage[dashboardID] = dashboard.Document.ToString();
                SaveToSession(storage);
            }
        }
    }
}
