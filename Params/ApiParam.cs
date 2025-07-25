using System;

namespace Params
{
    public class ApiParam
    {
        /// <summary>
        /// ApiResult 訊息類型
        /// </summary>
        public enum ApiMsgType
        {
            NONE = 0,
            INSERT = 1,
            UPDATE = 2,
            DELETE = 3,
            SAVE = 4
        }

        /// <summary>
        /// Enumerates the HTTP verbs.
        /// </summary>
        [Flags]
        public enum HttpVerbs
        {
            /// <summary>
            /// Retrieves the information or entity that is identified by the URI of the request.
            /// </summary>
            Get = 1,
            /// <summary>
            /// Posts a new entity as an addition to a URI.
            /// </summary>
            Post = 2,
            /// <summary>
            /// Replaces an entity that is identified by a URI.
            /// </summary>
            Put = 4,
            /// <summary>
            /// Requests that a specified URI be deleted.
            /// </summary>
            Delete = 8,
            /// <summary>
            /// Retrieves the message headers for the information or entity that is identified by the URI of the request.
            /// </summary>
            Head = 16,
            /// <summary>
            /// Requests that a set of changes described in the request entity be applied to the resource identified by the Request- URI.
            /// </summary>
            Patch = 32,
            /// <summary>
            /// Represents a request for information about the communication options available on the request/response chain identified by the Request-URI.
            /// </summary>
            Options = 64
        }
    }
}
