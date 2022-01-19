using ProcessLayer.Entities;
using ProcessLayer.Processes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ProcessLayer.Helpers
{
    public static class Web
    {
        public static bool IsFileSupported(string file)
        {
            return new List<string> {
                    ".jpeg",
                    ".png",
                    ".jpg",
                    ".gif",
                    ".tiff",
                    ".psd",
                    ".pdf",
                    ".eps",
                    ".ai",
                    ".indd",
                    ".raw"
                }.Contains(Path.GetExtension(file));
        }
        public static List<int> PerpetialLicenseIDs(IEnumerable<LicenseType> licensetype)
        {
            return licensetype.Where(x => x.Perpetual).Select(x => x.ID).ToList();
        }

        public static List<int> NoCourseIDs(IEnumerable<Lookup> educationalLevel)
        {
            return educationalLevel.Where(x => x.Description.ToLower().Contains("primary") || x.Description.ToLower().Contains("secondary")).Select(x => x.ID).ToList();
        }

        public static int GetMemoTypeMemoId()
        {
            var memoType = LookupProcess.GetMemoType();
            foreach(var m in memoType)
            {
                if (m.Description.ToUpper().Contains("MEMO"))
                    return m.ID;
            }
            return 0;
        }

        public static int GetMemoTypeInfractionId()
        {
            var memoType = LookupProcess.GetMemoType();
            foreach (var m in memoType)
            {
                if (m.Description.ToUpper().Contains("INFRACTION"))
                    return m.ID;
            }
            return 0;
        }

        public static List<Lookup> YearList()
        {
            List<Lookup> year = new List<Lookup>();
            for (int i = DateTime.Now.Year; i >= 1920; i--)
            {
                year.Add(i, i.ToString());
            }
            return year;
        }

        public static List<Lookup> MonthList()
        {
            return new List<Lookup> { { 1, "January" }, { 2, "February" }, { 3, "March" }, { 4, "April" }, { 5, "May" }, { 6, "June" }, { 7, "July" }, { 8, "August" }, { 9, "September" }, { 10, "October" }, { 11, "November" }, { 12, "December" } };
        }

        public static EmailCredential GetMemoEmailCreadential()
        {
            return EmailCredentialProcess.Get("MEMO");
        }
        public static bool HasInternetConnection()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
