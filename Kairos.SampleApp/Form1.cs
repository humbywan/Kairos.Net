using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Kairos.API.KairosClient client = new Kairos.API.KairosClient();

            client.ApplicationID = "c4214740";
            client.ApplicationKey = "64cbdf468dc6a3523e4393b63735cdcf";

            // Detect the face(s)
            var detectResponse = client.Detect("http://wellness.18signals.com/kairos.jpg");

            // Get the image and face information
            var detectImage = detectResponse.Images[0];
            var face = detectImage.Faces[0];

            // Enroll the user
            var enrollResponse = client.Enroll(detectImage.image_id, "humbywan1234", face.topLeftX, face.topLeftY, face.width, face.height);

            // Get the user enrollment transaction info
            var userImage = enrollResponse.Images[0].Transaction;

            // Recognize the user
            var user = client.Recognize(userImage.image_id, face.topLeftX, face.topLeftY, face.width, face.height);

            // Detected user ID
            var userID = user.Images[0].Candidates.First().Key;        
        }
    }
}
