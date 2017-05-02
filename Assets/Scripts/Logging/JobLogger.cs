using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections.Generic;
using System.IO;

namespace PickAR.Logging {

    /// <summary>
    /// Logs job execution history, including the time taken between each objects.
    /// </summary>
    public class JobLogger : MonoBehaviour {

        /// <summary> The date that the job started at. </summary>
        private DateTime startTime;

        /// <summary> The number of seconds that the last job event occurred at. </summary>
        private float lastTime;

        /// <summary> The items that have been collected. </summary>
        private List<LoggedItem> items;

        /// <summary> The folder to store log files in. </summary>
        [SerializeField]
        [Tooltip("The folder to store log files in.")]
        private string logPath;

        /// <summary>
        /// Starts logging a job.
        /// </summary>
        public void StartLog() {
            startTime = DateTime.Now;
            lastTime = Time.time;
            items = new List<LoggedItem>();
        }

        /// <summary>
        /// Logs the time that it took to collect an item.
        /// </summary>
        /// <param name="itemID">The ID of the item that was collected.</param>
        public void LogItem(int itemID) {
            float currentTime = Time.time;
            LoggedItem newItem = new LoggedItem(itemID, currentTime - lastTime);
            items.Add(newItem);
            lastTime = currentTime;
        }

        /// <summary>
        /// Ends logging for the current job and writes it to a JSON file.
        /// </summary>
        public void EndLog() {
            float timeToEnd = Time.time - lastTime;
            DateTime endTime = DateTime.Now;

            string startString = startTime.ToString();
            string logName = "Log_" + startString.Replace(":", "").Replace("/", "").Replace(" ", "_");

            string logFolder = Path.Combine(Application.streamingAssetsPath, logPath);
            string logFile = Path.Combine(logFolder, logName) + ".json";

            JSONObject json = new JSONObject(JSONObject.Type.OBJECT);
            json.AddField("startTime", startString);
            json.AddField("endTime", endTime.ToString());
            JSONObject itemsJSON = new JSONObject(JSONObject.Type.ARRAY);

            foreach (LoggedItem item in items) {
                JSONObject itemJSON = new JSONObject(JSONObject.Type.OBJECT);
                itemJSON.AddField("id", item.itemID);
                itemJSON.AddField("timeTaken", item.timeTaken);
                itemsJSON.Add(itemJSON);
            }
            JSONObject endJSON = new JSONObject(JSONObject.Type.OBJECT);
            endJSON.AddField("timeTaken", timeToEnd);
            itemsJSON.Add(endJSON);
            json.AddField("items", itemsJSON);

            if (!Directory.Exists(logFolder)) {
                Directory.CreateDirectory(logFolder);
            }
            File.WriteAllText(logFile, json.Print(true));
        }
    }

    /// <summary>
    /// Data for an item that is kept track of for logging.
    /// </summary>
    struct LoggedItem {

        /// <summary> The ID of the logged item. </summary>
        internal float itemID;
        /// <summary> The amount of time (seconds) after the previous item that it took to collect this item. </summary>
        internal float timeTaken;

        /// <summary>
        /// Initializes a new logged item.
        /// </summary>
        /// <param name="itemID">The ID of the logged item.</param>
        /// <param name="timeTaken">The amount of time (seconds) after the previous item that it took to collect this item.</param>
        internal LoggedItem(float itemID, float timeTaken) {
            this.itemID = itemID;
            this.timeTaken = timeTaken;
        }
    }
}