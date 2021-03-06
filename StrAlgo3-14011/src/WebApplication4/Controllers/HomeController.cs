﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TweetSharp;

namespace WebApplication4.Controllers
{  
    public class HomeController : Controller
    {
        static void buildTable(ref string keyword, ref List<int> table)
        {
            int i = 2;
            int j = 0;
            table.Add(-1);
            table.Add(0);

            while (i < keyword.Length)
            {
                if (keyword[i-1] == keyword[j])
                {
                    table.Insert(i, j + 1);
                    i++;
                    j++;
                }
                else if (j > 0)
                {
                    j = table[j];
                }
                else
                {
                    table.Insert(i, 0);
                    i++;
                }
            }
        }

        static int KMP(string text, string keyword)
        // mengembalikan indeks posisi ditemukan, mengembalikan panjang text jika keyword tidak ditemukan 
        {
            int i = 0; // indeks untuk teks
            int j = 0; // indeks untuk keyword
            List<int> table = new List<int>(keyword.Length);

            buildTable(ref keyword, ref table);
            while (i + j < text.Length)
            {
                if (keyword[j] == text[i + j])
                {
                    j++;
                    if (j == keyword.Length)
                        return i;
                }
                else
                {
                    i += j - table[j];
                    if (j > 0)
                        j = table[j];
                }
            }
            return text.Length;
        }
        
        static void compute_last(ref string keyword, ref Dictionary<int, int> lastPosition)
        {
            for (int i = 0; i < keyword.Length; i++)
            {
                if (lastPosition.ContainsKey(keyword[i]))
                {
                    lastPosition[keyword[i]] = i + 1;
                }
                else
                {
                    lastPosition.Add(keyword[i], i + 1);
                }
            }
        }

        static int BM(string text, string keyword)
        // mengembalikan indeks posisi ditemukan, mengembalikan panjang text jika keyword tidak ditemukan 
        {
            Dictionary<int, int> lastPosition = new Dictionary<int, int>();
            compute_last(ref keyword, ref lastPosition);

            int textLength = text.Length;
            int keywordLength = keyword.Length;
            int i = 0;

            while (i <= textLength - keywordLength)
            {
                int j = keywordLength - 1;

                // mundurkan indeks j selama karakter sesuai
                while ((j >= 0) && keyword[j] == text[i + j])
                    j--;

                // keyword cocok
                if (j == -1)
                    return i;
                else // keyword tidak cocok
                {
                    int mismatch = (int)text[i + j];
                    if (lastPosition.ContainsKey(mismatch))
                    {
                        if (j < lastPosition[mismatch])
                            i++;
                        else
                            i = i + j - lastPosition[mismatch] + 1;
                    }
                    else
                    {
                        i = i + j + 1;
                    }
                }
            }
            return text.Length;
        }
        
        string getLocation(string text)
        {
            string s = "";
            char temp = ' ';
            int itr = KMP(text, " di ");
            itr++;
            while (!text[itr].Equals(temp))
            {
                itr++;
            }
            itr++;
            while (!text[itr].Equals(temp))
            {
                s = s + text[itr];
                itr++;
            }
            s = s.ToLower();
            string[] stringTemp = { "jl", "jl.", "jalan", "kelurahan", "kecamatan", "kantor" };
            string[] stringTemp2 = { "itb", "cisitu", "tubagus", "cihampelas" };
            bool found = false;
            for (int i=0; i<6; i++)
            {
                if (s.Equals(stringTemp[i]))
                {
                    found = true;
                }
            }
            bool found2 = false;
            for (int j = 0; j < 4; j++)
            {
                if (s.Equals(stringTemp2[j]))
                {
                    found2 = true;
                }
            }
            if (found)
            {
                s = s + " ";
                itr++;
                while (!text[itr].Equals(temp))
                {
                    s = s + text[itr];
                    itr++;
                }
            }
            else if (found2) { }
            else
            {
                s = " ";
            }
            return s;
        }

        [HttpGet]
        public ActionResult About()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Form()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Form(string txtTwitterName, string txtboxPDAM, string txtboxPJU, string txtboxDinsos, string txtboxDBMP, string txtboxDiskom, string txtboxPDBersih)
        {
            TempData["QueryPDAM"] = txtboxPDAM;
            TempData["QueyDiskom"] = txtboxDiskom;
            TempData["QueryPDBersih"] = txtboxPDBersih;
            TempData["QueryDBMP"] = txtboxDBMP;
            TempData["QueryPJU"] = txtboxPJU;
            TempData["QueryDinsos"] = txtboxDinsos;
            TempData["Tweetsearch"] = txtTwitterName;
            TempData["searchingMethod"] = Request.Form["searchingMethod"];

            return RedirectToAction("Result");
        }
        
        public ActionResult Result()
        {
            string consumerKey = "DShcBlKUwZ3mUBRt3tjqJEDnd";
            string consumerSecret = "GxJYqCJTcJyteeqSDg0sXC3xRxe4ydbbSljbSxfUvv7Z9TiROq";
            string accessToken = "2535482743-roYx8Hbx0qqdluKQFeIQlVOKvmp5IVUXaO5z9b3";
            string accessTokenSecret = "OGuX0xXjB9BiEw1K3Htgpm2LbeRUsh2EqBRf3k7TXi4VT";
            
            string tweetSearchQuery = (string)TempData["Tweetsearch"];
            string searchMethod = (string)TempData["searchingMethod"];
            string key1 = (string)TempData["QueryPDAM"];
            string key2 = (string)TempData["QueryPJU"];
            string key3 = (string)TempData["QueryDinsos"];
            string key4 = (string)TempData["QueryDBMP"];
            string key5 = (string)TempData["QueyDiskom"];
            string key6 = (string)TempData["QueryPDBersih"];

            int count1 = 0;
            int count2 = 0;
            int count3 = 0;
            int count4 = 0;
            int count5 = 0;
            int count6 = 0;
            int count7 = 0;

            if (TempData["Tweetsearch"] != null)
            {
                var service = new TwitterService(consumerKey, consumerSecret);
                service.AuthenticateWith(accessToken, accessTokenSecret);
                
                string[] tweetSQ = tweetSearchQuery.Split(';');
                int i;
                tweetSearchQuery = tweetSQ[0];
                for (i = 1; i < tweetSQ.Length; i++)
                {
                    tweetSearchQuery =  tweetSearchQuery + " OR " + tweetSQ[i];
                }

                TwitterSearchResult hasil = service.Search(new SearchOptions { Q = tweetSearchQuery, Count = 100 });
                IEnumerable<TwitterStatus> tweets = hasil.Statuses;
                ViewBag.Tweets = tweets;
                
                List<TwitterStatus> tweets1 = new List<TwitterStatus>();
                List<TwitterStatus> tweets2 = new List<TwitterStatus>();
                List<TwitterStatus> tweets3 = new List<TwitterStatus>();
                List<TwitterStatus> tweets4 = new List<TwitterStatus>();
                List<TwitterStatus> tweets5 = new List<TwitterStatus>();
                List<TwitterStatus> tweets6 = new List<TwitterStatus>();
                List<TwitterStatus> tweets7 = new List<TwitterStatus>();
                
                List<String> location1 = new List<String>();
                List<String> location2 = new List<String>();
                List<String> location3 = new List<String>();
                List<String> location4 = new List<String>();
                List<String> location5 = new List<String>();
                List<String> location6 = new List<String>();
                List<String> location7 = new List<String>();

                int posisi = 0;
                int min, minIdx=0;
                string text;
                List<TwitterStatus> ltweet = tweets.ToList();
                
                foreach (var tweet in ltweet)
                {
                    string[] words;
                    char[] delimiterChars = { ';' };
                    text = tweet.Text;
                    min = text.Length;
                    
                    words = key1.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string phrase in words)
                    {
                        int minTemp = min;
                        int minIdxTemp = minIdx;
                        string[] _words;
                        _words = phrase.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string keyWord in _words)
                        {
                            if (searchMethod == "KMP")
                            {
                                posisi = KMP(text.ToLower(), keyWord.ToLower());
                            }
                            else
                            {
                                posisi = BM(text.ToLower(), keyWord.ToLower());
                            }
                            if (posisi == text.Length)
                            {
                                min = minTemp;
                                minIdx = minIdxTemp;
                                break;
                            }
                            if (posisi < min)
                            {
                                min = posisi;
                                minIdx = 1;
                                continue;
                            }
                        }
                    }

                    words = key2.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string phrase in words)
                    {
                        int minTemp = min;
                        int minIdxTemp = minIdx;
                        string[] _words;
                        _words = phrase.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string keyWord in _words)
                        {
                            if (searchMethod == "KMP")
                            {
                                posisi = KMP(text.ToLower(), keyWord.ToLower());
                            }
                            else
                            {
                                posisi = BM(text.ToLower(), keyWord.ToLower());
                            }
                            if (posisi == text.Length)
                            {
                                min = minTemp;
                                minIdx = minIdxTemp;
                                break;
                            }
                            if (posisi < min)
                            {
                                min = posisi;
                                minIdx = 2;
                                continue;
                            }
                        }
                    }

                    words = key3.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string phrase in words)
                    {
                        int minTemp = min;
                        int minIdxTemp = minIdx;
                        string[] _words;
                        _words = phrase.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string keyWord in _words)
                        {
                            if (searchMethod == "KMP")
                            {
                                posisi = KMP(text.ToLower(), keyWord.ToLower());
                            }
                            else
                            {
                                posisi = BM(text.ToLower(), keyWord.ToLower());
                            }
                            if (posisi == text.Length)
                            {
                                min = minTemp;
                                minIdx = minIdxTemp;
                                break;
                            }
                            if (posisi < min)
                            {
                                min = posisi;
                                minIdx = 3;
                                continue;
                            }
                        }
                    }

                    words = key4.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string phrase in words)
                    {
                        int minTemp = min;
                        int minIdxTemp = minIdx;
                        string[] _words;
                        _words = phrase.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string keyWord in _words)
                        {
                            if (searchMethod == "KMP")
                            {
                                posisi = KMP(text.ToLower(), keyWord.ToLower());
                            }
                            else
                            {
                                posisi = BM(text.ToLower(), keyWord.ToLower());
                            }
                            if (posisi == text.Length)
                            {
                                min = minTemp;
                                minIdx = minIdxTemp;
                                break;
                            }
                            if (posisi < min)
                            {
                                min = posisi;
                                minIdx = 4;
                                continue;
                            }
                        }
                    }

                    words = key5.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string phrase in words)
                    {
                        int minTemp = min;
                        int minIdxTemp = minIdx;
                        string[] _words;
                        _words = phrase.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string keyWord in _words)
                        {
                            if (searchMethod == "KMP")
                            {
                                posisi = KMP(text.ToLower(), keyWord.ToLower());
                            }
                            else
                            {
                                posisi = BM(text.ToLower(), keyWord.ToLower());
                            }
                            if (posisi == text.Length)
                            {
                                min = minTemp;
                                minIdx = minIdxTemp;
                                break;
                            }
                            if (posisi < min)
                            {
                                min = posisi;
                                minIdx = 5;
                                continue;
                            }
                        }
                    }
                    
                    words = key6.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string phrase in words)
                    {
                        int minTemp = min;
                        int minIdxTemp = minIdx;
                        string[] _words;
                        _words = phrase.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string keyWord in _words)
                        {
                            if (searchMethod == "KMP")
                            {
                                posisi = KMP(text.ToLower(), keyWord.ToLower());
                            }
                            else
                            {
                                posisi = BM(text.ToLower(), keyWord.ToLower());
                            }
                            if (posisi == text.Length)
                            {
                                min = minTemp;
                                minIdx = minIdxTemp;
                                break;
                            }
                            if (posisi < min)
                            {
                                min = posisi;
                                minIdx = 6;
                                continue;
                            }
                        }
                    }

                    if (min == text.Length)
                    {
                        minIdx = 7;
                    }
                    
                    string s;
                    if (minIdx == 1)
                    {
                        tweets1.Add(tweet);
                        count1++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location1.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location1.Add(s);
                        }
                    }
                    else if (minIdx == 2)
                    {
                        tweets2.Add(tweet);
                        count2++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location2.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location2.Add(s);
                        }
                    }
                    else if (minIdx == 3)
                    {
                        tweets3.Add(tweet);
                        count3++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location3.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location3.Add(s);
                        }
                    }
                    else if (minIdx == 4)
                    {
                        tweets4.Add(tweet);
                        count4++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location4.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location4.Add(s);
                        }
                    }
                    else if (minIdx == 5)
                    {
                        tweets5.Add(tweet);
                        count5++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location5.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location5.Add(s);
                        }
                    }
                    else if (minIdx == 6)
                    {
                        tweets6.Add(tweet);
                        count6++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location6.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location6.Add(s);
                        }
                    }
                    else
                    {
                        tweets7.Add(tweet);
                        count7++;
                        if (KMP(text, " di ") == text.Length)
                        {
                            location7.Add(" ");
                        }
                        else
                        {
                            s = getLocation(text);
                            location7.Add(s);
                        }
                    }
                }

                ViewBag.Tweets1 = tweets1;
                ViewBag.Tweets2 = tweets2;
                ViewBag.Tweets3 = tweets3;
                ViewBag.Tweets4 = tweets4;
                ViewBag.Tweets5 = tweets5;
                ViewBag.Tweets6 = tweets6;
                ViewBag.Tweets7 = tweets7;
                
                ViewBag.location1 = location1;
                ViewBag.location2 = location2;
                ViewBag.location3 = location3;
                ViewBag.location4 = location4;
                ViewBag.location5 = location5;
                ViewBag.location6 = location6;
                ViewBag.location7 = location7;
            }
            ViewBag.PDAM = (string)TempData["QueryPDAM"];
            ViewBag.PJU = (string)TempData["QueryPJU"];
            ViewBag.Dinsos = (string)TempData["QueryDinsos"];
            ViewBag.Diskom = (string)TempData["QueyDiskom"];
            ViewBag.PDBersih = (string)TempData["QueryPDBersih"];
            ViewBag.DBMP = (string)TempData["QueryDBMP"];

            string c1 = count1.ToString(); ViewBag.count1 = (string)c1;
            string c2 = count2.ToString(); ViewBag.count2 = (string)c2;
            string c3 = count3.ToString(); ViewBag.count3 = (string)c3;
            string c4 = count4.ToString(); ViewBag.count4 = (string)c4;
            string c5 = count5.ToString(); ViewBag.count5 = (string)c5;
            string c6 = count6.ToString(); ViewBag.count6 = (string)c6;
            string c7 = count7.ToString(); ViewBag.count7 = (string)c7;

            return View();
        }
    }
}