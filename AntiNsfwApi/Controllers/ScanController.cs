using Microsoft.AspNetCore.Mvc;
using NsfwSpyNS;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AntiNsfwApi.Controllers
{
    public class Prediction
    {
        public float percentage
        {
            get;
            set;
        }

        public bool nsfw
        {
            get;
            set;
        }

        public object extended
        {
            get;
            set;
        }
    }

    public class PostToScan
    {
        public System.Uri url
        {
            get;
            set;
        }
    }

    [ApiController]
    [Route("/scan")]
    public class ScanController : ControllerBase
    {
        private readonly NsfwSpy spy = new();

        private float getPercentage (NsfwSpyResult res)
        {
            if (res.PredictedLabel == "Pornography")
            {
                return res.Pornography;
            }
            else if (res.PredictedLabel == "Sexy")
            {
                return res.Sexy;
            }
            else if (res.PredictedLabel == "Hentai")
            {
                return res.Hentai; ;
            }
            else
            {
                return res.Neutral;
            }
        }

        [HttpPost]
        [Route("/scan/image")]
        public async Task<Prediction> ScanImage([FromBody]PostToScan info, [FromQuery(Name = "debug")] bool debug)
        {
            Prediction p = new();

            var res = await spy.ClassifyImageAsync(info.url);

            if (res.PredictedLabel == "Sexy")
            {
                p.nsfw = false;
                p.percentage = 0;
            } else
            {
                p.nsfw = res.IsNsfw;
                p.percentage = getPercentage(res);
            }

            if (debug)
            {
                p.extended = res;
            }

            return p;
        }

/*        [HttpPost]
        [Route("/scan/video")]
        public async Task<Prediction> ScanVideo([FromBody] PostToScan info, [FromQuery(Name = "debug")] bool debug)
        {
            Prediction p = new();

            var res = await spy.ClassifyVideoAsync(info.url);

            p.nsfw = res.IsNsfw;
            p.percentage = 1;
            if (debug)
            {
                p.extended = res;
            }

            return p;
        }*/
    }
}
