using Newtonsoft.Json.Linq;
using Shouldly;

namespace Samples.Server.Tests
{
    internal static class ShouldBeExtensions
    {
        /// <summary>
        /// Compares two strings after normalizing any encoding differences first.
        /// </summary>
        /// <param name="actual">Actual.</param>
        /// <param name="expected">Expected.</param>
        /// <param name="ignoreExtensions">
        /// Pass true to ignore the `extensions` path of the JSON when comparing for equality.
        /// </param>
        public static void ShouldBeEquivalentJson(this string actual, string expected, bool ignoreExtensions)
        {
            if (ignoreExtensions)
            {
                if (actual.StartsWith('['))
                {
                    var json = JArray.Parse(actual);
                    foreach (var item in json)
                    {
                        if (item is JObject obj)
                        {
                            obj.Remove("extensions");
                        }
                    }
                    actual = json.ToString(Newtonsoft.Json.Formatting.None);
                }
                else
                {
                    var json = JObject.Parse(actual);
                    json.Remove("extensions");
                    actual = json.ToString(Newtonsoft.Json.Formatting.None);
                }
            }

            actual.ShouldBeEquivalentJson(expected);
        }

        /// <summary>
        /// Compares two strings after normalizing any encoding differences first.
        /// </summary>
        /// <param name="actualJson">Actual.</param>
        /// <param name="expectedJson">Expected.</param>
        public static void ShouldBeEquivalentJson(this string actualJson, string expectedJson)
        {
            expectedJson = expectedJson.NormalizeJson();
            actualJson = actualJson.NormalizeJson();

            actualJson.ShouldBe(expectedJson);
        }

        private static string NormalizeJson(this string json) => json.Replace("'", @"\u0027").Replace("\"", @"\u0022");
    }
}
