// 
// WaveWriter.cs
//  
// Author:
//       Geza Kovacs <gkovacs@mit.edu>
// 
// Copyright (c) 2009 Geza Kovacs
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.IO;
using System.Collections.Generic;
using Arbingersys.Audio.Aumplib;

namespace praatinvoke
{	
	public class WaveWriter
	{
		public CallPraatDelegate callpraat;
		public int NUM_CHANNELS = 1;
		public int SAMPLE_RATE = 44100;
		public double SCALEPOWER = 1.0;
		public double SILENCETHRESHOLD = 1.0;
		public int MAGNIFICATION = 10;
		public double BACKGROUND = 30.0;
		public uint FRAMESPERBUFFER = 1024;
		public int PAUSECOUNTDOWN = 12;
		public int PAUSECOUNTUP = 5;
		
		public int sndcapnum;
		public int pauseCountup;
		public int pauseCountdown;
		public IntPtr soundf;
		LibsndfileWrapper.SF_INFO soundfInfo;
		public List<float> upcomingSoundCache = new List<float>();
		
		public void ReceiveSamples(float[] inpSamples)
		{
			try
			{
				double inpvecsum = 0.0;
				foreach (float sample in inpSamples)
				{
					inpvecsum += MAGNIFICATION * Math.Abs(sample);
				}
				inpvecsum /= SILENCETHRESHOLD;
				inpvecsum -= BACKGROUND;
				Console.WriteLine(inpvecsum.ToString("f10"));
				if (pauseCountup == 0) // is recording
				{
					Console.WriteLine("recording");
					if (inpvecsum > 0) // have sound, recording as usual
					{
						Console.WriteLine("have sound");
						pauseCountdown = PAUSECOUNTDOWN;
						writeSampleBuffer(inpSamples);
					}
					else
					{
						if (pauseCountdown == 0) // hasn't had sound input for countdown turns, stop recording
						{
							pauseCountdown = PAUSECOUNTDOWN;
							pauseCountup = PAUSECOUNTUP;
							changeSoundFile();
						}
						else // no sound but still recording
						{
							--pauseCountdown;
							writeSampleBuffer(inpSamples);
						}
					}
				}
				else
				{
					if (inpvecsum > 0) // not recording but have sound
					{
						upcomingSoundCache.AddRange(inpSamples);
						--pauseCountup;
						if (pauseCountup == 0) // have hit critical point, will now start recording, clear buffer and write to file
						{
							Console.WriteLine("have hit critical point");
							writeSampleBuffer(upcomingSoundCache.ToArray());
							upcomingSoundCache.Clear();
						}
					}
					else // no sound, reset pauseCountup and clear cache
					{
						Console.WriteLine("no sound");
						upcomingSoundCache.Clear();
						pauseCountup = PAUSECOUNTUP;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
		
		public void changeSoundFile()
		{
			try
			{
				Console.WriteLine("soundfile changed");
				string origsndcapfile = Environment.CurrentDirectory+Path.DirectorySeparatorChar+"sndcap"+Path.DirectorySeparatorChar+sndcapnum.ToString()+".wav";
				IntPtr soundfold = soundf;
				string sndcapfile = nextSoundFile();
				soundf = LibsndfileWrapper.sf_open(sndcapfile, (int)LibsndfileWrapper.fileMode.SFM_WRITE, ref soundfInfo);
				LibsndfileWrapper.sf_close(soundfold);
				callpraat(origsndcapfile);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
		
		public void writeSampleBuffer(float[] samples)
		{
			try
			{
				LibsndfileWrapper.sf_write_float(soundf, samples, samples.Length);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
		
		public string nextSoundFile()
		{
			try
			{
				while (File.Exists(Environment.CurrentDirectory+Path.DirectorySeparatorChar+"sndcap"+Path.DirectorySeparatorChar+sndcapnum.ToString()+".wav"))
					++sndcapnum;
				return Environment.CurrentDirectory+Path.DirectorySeparatorChar+"sndcap"+Path.DirectorySeparatorChar+sndcapnum.ToString()+".wav";
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}
		
		public void SetPraatDelegate(CallPraatDelegate pri)
		{
			try
			{
				callpraat = pri;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
		
		public ReceiveSamplesDelegate GetSamplesDelegate()
		{
			try
			{
				return new ReceiveSamplesDelegate(ReceiveSamples);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}
		
		public WaveWriter()
		{
			try
			{
				if (!Directory.Exists(Environment.CurrentDirectory+Path.DirectorySeparatorChar+"sndcap"))
				{
					Directory.CreateDirectory(Environment.CurrentDirectory+Path.DirectorySeparatorChar+"sndcap");
				}
				soundfInfo.channels = NUM_CHANNELS;
				soundfInfo.samplerate = SAMPLE_RATE;
				soundfInfo.format = ((int)LibsndfileWrapper.soundFormat.SF_FORMAT_WAV | (int)LibsndfileWrapper.soundFormat.SF_FORMAT_FLOAT);
				string sndcapfile = nextSoundFile();
				soundf = LibsndfileWrapper.sf_open(sndcapfile, (int)LibsndfileWrapper.fileMode.SFM_WRITE, ref soundfInfo);
				pauseCountup = PAUSECOUNTUP;
				pauseCountdown = PAUSECOUNTDOWN;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
