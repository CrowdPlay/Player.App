using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using CrowdPlay.Models;
using RestSharp;

namespace CrowdPlay.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var moods = GetAllMoods();

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Moods = moods.OrderBy(s => s).ToArray();
                var twitterHandle = GetTwitterHandle();

                ViewBag.TwitterHandle = twitterHandle;
                var user = GetUser(twitterHandle);

                ViewBag.Room = user.Room;
                ViewBag.Mood = user.Mood;
                ViewBag.OtherUsers = GetRoomInfo(user.Room).Usernames;
            }
            
            return View();
        }

        public ActionResult UpdateMood()
        {
            var json = new StreamReader(Request.InputStream).ReadToEnd();

            var requestUser = new JavaScriptSerializer().Deserialize<RequestUser>(json);

            var request = new RestRequest("/user", Method.PUT) { RequestFormat = DataFormat.Json };
            request.AddBody(requestUser);

            new RestClient("http://191.238.115.5:8081").Execute(request);

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private static RequestUser GetUser(string twitterHandle)
        {
            var request = new RestRequest("/user/" + twitterHandle, Method.GET) { RequestFormat = DataFormat.Json };

            var response = new RestClient("http://191.238.115.5:8081").Execute(request).Content;

            return new JavaScriptSerializer().Deserialize<RequestUser>(response);
        }

        private static string GetTwitterHandle()
        {
            var prinicpal = (ClaimsPrincipal)Thread.CurrentPrincipal;
            return prinicpal.Claims.First(claim => claim.Type == ("urn:twitter:screen_name")).Value;
        }

        private IEnumerable<string> GetAllMoods()
        {
            var request = new RestRequest("GetAllMood/", Method.GET) { RequestFormat = DataFormat.Json };

            var response = new RestClient("http://191.238.115.5:8081").Execute(request).Content;

            return new JavaScriptSerializer().Deserialize<IEnumerable<string>>(response);
        }

        private RoomUserModel GetRoomInfo(int roomId)
        {
            var request = new RestRequest("/roomInfo/" + roomId, Method.GET) { RequestFormat = DataFormat.Json };

            var response = new RestClient("http://191.238.115.5:8081").Execute(request).Content;

            return new JavaScriptSerializer().Deserialize<RoomUserModel>(response);
        }

        [HttpGet]
        public ActionResult NowPlaying(int roomId)
        {
            var roomInfo = DoRoomInfoRequest(roomId);
            var trackDetail = DoTrackDetailRequest(roomInfo.trackId);
            var artistDetail = DoArtistDetailRequest(trackDetail.Artist.ID);
            var releaseDetail = DoReleaseDetailRequest(trackDetail.Release.ID);

            var nowPlayingModel = new NowPlayingModel();

            nowPlayingModel.Track = trackDetail.Track;
            nowPlayingModel.Artist = artistDetail.Artist;
            nowPlayingModel.Release = releaseDetail.Release;
        
            return Json(nowPlayingModel, JsonRequestBehavior.AllowGet);
        }

        private static RoomInfoModel DoRoomInfoRequest(int roomId)
        {
            var request = new RestRequest("/room/{roomId}/info");
            request.AddUrlSegment("roomId", roomId.ToString());

            var client = new RestClient("http://crowdplay-streamer.azurewebsites.net");
            var response = client.Execute<RoomInfoModel>(request);
            return response.Data;
        }

        private static TrackDetailModel DoTrackDetailRequest(int trackId)
        {
            var request = new RestRequest("/track/{trackId}");
            request.AddUrlSegment("trackId", trackId.ToString());

            var client = new RestClient("http://crowdplay-streamer.azurewebsites.net");
            var response = client.Execute<TrackDetailModel>(request);
            return response.Data;
        }

        private static ArtistDetailModel DoArtistDetailRequest(int artistId)
        {
            var trackDetailRequest = new RestRequest("/artist/{artistId}");
            trackDetailRequest.AddUrlSegment("artistId", artistId.ToString());

            var client = new RestClient("http://crowdplay-streamer.azurewebsites.net");
            var trackDetailResponse = client.Execute<ArtistDetailModel>(trackDetailRequest);
            return trackDetailResponse.Data;
        }

        private static ReleaseDetailModel DoReleaseDetailRequest(int releaseId)
        {
            var trackDetailRequest = new RestRequest("/release/{releaseId}");
            trackDetailRequest.AddUrlSegment("releaseId", releaseId.ToString());

            var client = new RestClient("http://crowdplay-streamer.azurewebsites.net");
            var trackDetailResponse = client.Execute<ReleaseDetailModel>(trackDetailRequest);
            return trackDetailResponse.Data;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }

    public class RoomUserModel
    {
        public IEnumerable<string> Usernames { get; set; }
    }

    public class RoomInfoModel
    {
        public int id { get; set; }
        public int trackId { get; set; }
    }

    public class ReleaseDetailModel
    {
        public ReleaseDetail Release { get; set; }
    }

    public class ReleaseDetail
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public string Image { get; set; }
        public DateTime ReleaseDate { get; set; }
    }

    public class ArtistDetailModel
    {
        public ArtistDetail Artist { get; set; }
    }

    public class ArtistDetail
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
    }

    public class TrackDetailModel
    {
        public TrackDetail Track { get; set; }
        public ArtistDetail Artist { get; set; }
        public ReleaseDetail Release { get; set; }
    }

    public class TrackDetail
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int TrackNumber { get; set; }
    }

    public class NowPlayingModel
    {
        public TrackDetail Track { get; set; }
        public ArtistDetail Artist { get; set; }
        public ReleaseDetail Release { get; set; }
    }
}