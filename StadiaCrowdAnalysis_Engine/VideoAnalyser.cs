/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Vision.Motion;
using Accord.Imaging;
using Accord.Video.DirectShow;
using System.Drawing;
using System.Threading;

namespace StadiaCrowdAnalysis_Engine
{
    public class VideoAnalyser
    {
        #region Variables
        private String _fileName;
        private FileVideoSource _video;
        private bool _firstFrameFlag;
        private Bitmap _previousFrame;
        private Bitmap _currentFrame;
        private AutoResetEvent _doneEvent;
        #endregion

        public VideoAnalyser()
        {
            FileName = "";
            _firstFrameFlag = false;
        }

        #region Properties
        public String FileName { set { _fileName = value; } }

        public Bitmap CurrentFrame { get { return _currentFrame; } }

        public Bitmap PreviousFrame { get { return _previousFrame; } }
        #endregion

        #region Methods
        public void LoadFile()
        {
            if (_fileName == null || _fileName.Equals("")) return;

            try
            {
                _doneEvent = new AutoResetEvent(false);

                _video = new FileVideoSource(_fileName);
                _video.NewFrame += new Accord.Video.NewFrameEventHandler(Video_NewFrame);
                _video.PlayingFinished += Video_PlayingFinished;
                
                _video.Start();

                _firstFrameFlag = true;

                _doneEvent.WaitOne();

                _firstFrameFlag = false;
            }
            catch { }
        }
        
        #endregion

        #region Events
        private void Video_NewFrame(object sender, Accord.Video.NewFrameEventArgs eventArgs)
        {
            if(_currentFrame != null) _previousFrame = _currentFrame; //Update the frames
            _currentFrame = new Bitmap(eventArgs.Frame); //Create a new copy so that when eventArgs is disposed it doesn't take the frame reference with it
            if(_firstFrameFlag)
                _video.SignalToStop(); //Get the first frame only to begin with for any preprocess necessary
        }

        private void Video_PlayingFinished(object sender, Accord.Video.ReasonToFinishPlaying reason)
        {
            _firstFrameFlag = false;
            _doneEvent.Set();
        }
        #endregion
    }
}
