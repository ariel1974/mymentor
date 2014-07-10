using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMentor.ParseObjects;
using Parse;

namespace MyMentor
{
    public static class ParseTables
    {
        public static ParseObject CurrentUser;

        public static async Task<ParseObject> GetContentType()
        {
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            var q = await (from user in ParseUser.Query
                           where user.ObjectId == ParseUser.CurrentUser.ObjectId
                           select user).FirstAsync();

            ParseTables.CurrentUser = q;

            Program.Logger.Info("current user: " + q.ObjectId);
            Program.Logger.Info("current content type: " + q.Get<ParseObject>("contentType").ObjectId);

            var query = await (from armor in ParseObject.GetQuery("WorldContentType")
                               where armor.ObjectId == q.Get<ParseObject>("contentType").ObjectId
            select armor).FirstAsync();

            // Stop timing
            stopwatch.Stop();

            // Write result
            Program.Logger.InfoFormat("Time elapsed GetContentType: {0}",
                stopwatch.Elapsed);

            return query;
        }

        public static async Task<IEnumerable<ParseObject>> GetCategory1(string contentType)
        {
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            //get category 1
            var query = await (from cat in ParseObject.GetQuery("Category1")
                               where
                               cat["contentType"] == ParseObject.CreateWithoutData("WorldContentType", contentType)
                               orderby cat.Get<int>("order")
                               select cat).FindAsync();

            // Stop timing
            stopwatch.Stop();

            // Write result
            Program.Logger.InfoFormat("Time elapsed GetCategory1: {0}",
                stopwatch.Elapsed);

            return query;
        }

        public static async Task<IEnumerable<ParseObject>> GetUsers()
        {
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            //get category 1
            var query = await (from user in ParseUser.Query
                               where user.Get<string>("username") != null
                               select user).FindAsync();

            // Stop timing
            stopwatch.Stop();

            // Write result
            Program.Logger.InfoFormat("Time elapsed GetCategory1: {0}",
                stopwatch.Elapsed);

            return query;
        }

        public static async Task<IEnumerable<KeyValuePair<string, string>>> GetStrings()
        {
            // Create new stopwatch
            Stopwatch stopwatch = new Stopwatch();

            // Begin timing
            stopwatch.Start();

            //get category 1
            var query = await (from cat in ParseObject.GetQuery("Strings")
                               where
                               cat.Get<string>("code") != null
                               select cat).FindAsync();

            // Stop timing
            stopwatch.Stop();

            // Write result
            Program.Logger.InfoFormat("Time elapsed Strings: {0}",
                stopwatch.Elapsed);

            return query.Select(s => new KeyValuePair<string, string>(s.Get<string>("code"), s.Get<string>(MyMentor.Properties.Settings.Default.CultureInfo.Replace("-","_"))));
        }

        public static async Task<IEnumerable<ParseObject>> GetCategory3(string contentType, string lessonType)
        {

            //get category 3
            var query = from cat in ParseObject.GetQuery("Category3")
                        where
                        cat["contentType"] == ParseObject.CreateWithoutData("WorldContentType", contentType)
                        && (cat["lessonType"] == ParseObject.CreateWithoutData("LessonTypes", "v0SqUiv5mr") || //all
                        cat["lessonType"] == ParseObject.CreateWithoutData("LessonTypes", lessonType))
                        orderby cat.Get<int>("order")
                        select cat;

            return await query.FindAsync();
        }

        public static async Task<IEnumerable<ParseObject>> GetVoicePrompts()
        {
            var query = from cat in ParseObject.GetQuery("VoicePrompts")
                        where
                        (cat["Teacher"] == ParseObject.CreateWithoutData("_User", ParseUser.CurrentUser.ObjectId) && !cat.Get<bool>("MyMentorVoice"))
                        || cat.Get<bool>("MyMentorVoice")
                        orderby cat.Get<int>("Sorting")
                        select cat;

            return await query.FindAsync();
        }

        public static async Task<ParseObject> GetCategoryLabels(string contentType)
        {

            //get category 3
            var query = from cat in ParseObject.GetQuery("CategoryLabels")
                        where
                        cat["contentType"] == ParseObject.CreateWithoutData("WorldContentType", contentType)
                        && cat.Get<string>("culture") == MyMentor.Properties.Settings.Default.CultureInfo
                        select cat;

            return await query.FirstAsync();
        }


        public static async Task<IEnumerable<ParseObject>> GetCategory4(string contentType)
        {

            //get category 3
            var query = from cat in ParseObject.GetQuery("Category4")
                        where
                        cat["contentType"] == ParseObject.CreateWithoutData("WorldContentType", contentType)
                        orderby cat.Get<int>("order")
                        select cat;

            return await query.FindAsync();
        }


        public static async Task<IEnumerable<ParseObject>> GetStatuses()
        {
            //get category 3
            var query = from sta in ParseObject.GetQuery("ClipStatus")
                        where sta.Get<bool>("isVisibleToMentor")
                        orderby sta.Get<int>("order")
                        select sta;

            return await query.FindAsync();
        }

        public static async Task<IEnumerable<ParseObject>> GetTypes()
        {
            //get category 3
            var query = from sta in ParseObject.GetQuery("ClipType")
                        where sta.Get<string>("value_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_")) != ""
                        select sta;

            return await query.FindAsync();
        }

        public static async Task<IEnumerable<ParseObject>> GetCategory2(string category1)
        {

            //get category 2
            var query = from cat in ParseObject.GetQuery("Category2")
                        where
                        cat["category1"] == ParseObject.CreateWithoutData("Category1", category1)
                        orderby cat.Get<int>("order")
                        select cat;

            return await query.FindAsync();
        }

    }
}
