/*************************************************************************
* 
* 18 SIGNALS CONFIDENTIAL
* __________________
* 
*  [2013] 18 SIGNALS, LLC
*  All Rights Reserved.
* 
* NOTICE:  All information contained herein is, and remains
* the property of 18 SIGNALS, LLC and its suppliers,
* if any.  The intellectual and technical concepts contained
* herein are proprietary to 18 SIGNALS, LLC and its suppliers 
* and may be covered by U.S. and Foreign Patents, patents in process, 
* and are protected by trade secret or copyright law.
* Dissemination of this information or reproduction of this material
* is strictly forbidden unless prior written permission is obtained
* from 18 SIGNALS, LLC.
*/

using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kairos.API
{
    /// <summary>
    /// Wrapper class responsible for making call to the Kairos API
    /// </summary>
    public class KairosClient
    {
        private const string API_BASE_URL = "http://api.kairos.io";

        private string _applicationID { get; set; }
        private string _applicationKey { get; set; }

        /// <summary>
        /// The application ID
        /// </summary>
        public string ApplicationID 
        {
            get { return this._applicationID; }
            set { this._applicationID = value; } 
        }
        
        /// <summary>
        /// The application key
        /// </summary>
        public string ApplicationKey         
        {
            get { return this._applicationKey; }
            set { this._applicationKey = value; }
        }

        /// <summary>
        /// Our default constructor taking the application ID and application key as parameters
        /// </summary>
        public KairosClient()
        {
            // Initialize internal members
            this._applicationID = null;
            this._applicationKey = null;
        }

        /// <summary>
        /// Our default constructor taking the application ID and application key as parameters
        /// </summary>
        /// <param name="applicationId">The Kairos application ID</param>
        /// <param name="applicationKey">The Kairos application Key</param>
        public KairosClient(string applicationId, string applicationKey)
        {
            // Initialize internal members
            this._applicationID = applicationId;
            this._applicationKey = applicationKey;
        }

        /// <summary>
        /// Detects images and faces from an image
        /// </summary>
        /// <param name="imageUrl">The location (URI) of the image</param>
        /// <returns>The response from the call to /Detect</returns>
        public Kairos.API.DetectResponse Detect(string imageUrl)
        {
            var client = new RestClient(API_BASE_URL);
            
            var request = new RestRequest("detect", Method.POST);

            // Set the parameters
            request.AddParameter("data", "{\"url\":\"" + imageUrl + "\"}");
            request.AddParameter("app_id", this._applicationID);
            request.AddParameter("app_key", this._applicationKey);
            
            // Specify the content type expected before desiarializing the object
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            // Execute the request
            var requestResponse = client.Execute<DetectResponse>(request);

            // Return the response
            return requestResponse.Data;
        }

        /// <summary>
        /// Enrolls a previously detected image into the system by specifying the subject ID
        /// </summary>
        /// <param name="imageId">The ID of the image previously detected</param>
        /// <param name="subjectId">The tracking ID of the user for which the image is being enrolled</param>
        /// <param name="topLeftX">The x coordinate of the face location</param>
        /// <param name="topLeftY">The y coordinate of the face location</param>
        /// <param name="width">The width of the image area</param>
        /// <param name="height">The height of the image area</param>
        /// <returns></returns>
        public Kairos.API.EnrollResponse Enroll(string imageId, string subjectId, int topLeftX, int topLeftY, int width, int height)
        {
            var client = new RestClient(API_BASE_URL);

            var request = new RestRequest("enroll", Method.POST);

            // Set the parameters
            request.AddParameter("data", "{\"image_id\":\"" + imageId + "\",\"subject_id\":\"" + subjectId + "\",\"topLeftX\":" + topLeftX + ",\"topLeftY\":" + topLeftY + ",\"width\":" + width + ",\"height\":" + height + "}");
            request.AddParameter("app_id", this._applicationID);
            request.AddParameter("app_key", this._applicationKey);

            // Specify the content type expected before desiarializing the object
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            // Execute the request
            var requestResponse = client.Execute<EnrollResponse>(request);

            // Return the response
            return requestResponse.Data;
        }

        /// <summary>
        /// Recognizes a previously detected/enrolled image in the system 
        /// </summary>
        /// <param name="imageId">The image ID</param>
        /// <param name="topLeftX">The x coordinate of the face location</param>
        /// <param name="topLeftY">The y coordinate of the face location</param>
        /// <param name="width">The width of the image area</param>
        /// <param name="height">The height of the image area</param>
        /// <returns>The recognition response with the possible matches</returns>
        public Kairos.API.RecognizeResponse Recognize(string imageId, int topLeftX, int topLeftY, int width, int height)
        {
            var client = new RestClient(API_BASE_URL);

            var request = new RestRequest("recognize", Method.POST);

            // Set the parameters
            request.AddParameter("data", "{\"image_id\":\"" + imageId + "\",\"topLeftX\":" + topLeftX + ",\"topLeftY\":" + topLeftY + ",\"width\":" + width + ",\"height\":" + height + "}");
            request.AddParameter("app_id", this._applicationID);
            request.AddParameter("app_key", this._applicationKey);

            // Specify the content type expected before desiarializing the object
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };

            // Execute the request
            var requestResponse = client.Execute<RecognizeResponse>(request);

            // Return the response
            return requestResponse.Data;
        }

        /// <summary>
        /// Recognizes a previously detected/enrolled image in the system 
        /// </summary>
        /// <param name="imageUrl">The image ID</param>
        /// <returns>The recognition response with the possible matches</returns>
        public Kairos.API.RecognizeResponse Recognize(string imageUrl)
        {
            // Detect the image information
            var detectResponse = this.Detect(imageUrl);

            // Get the image and face information
            var detectImage = detectResponse.Images[0];
            var face = detectImage.Faces[0];

            // Recognize faces
            return this.Recognize(detectImage.image_id, face.topLeftX, face.topLeftY, face.width, face.height);
        }
    }
}
