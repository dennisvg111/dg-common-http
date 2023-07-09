using System;

namespace DG.Common.Http.Redirections
{
    public class RedirectResponse
    {
        public RedirectType Type { get; set; }
        public Uri Location { get; set; }
    }
}
