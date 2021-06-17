using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace GraphQL.Server.Transports.AspNetCore.Tests
{
    public partial class NewtonsoftJsonTests
    {
        [Fact]
        public async Task Decodes_Request()
        {
            var request = @"{""query"":""abc"",""operationName"":""def"",""variables"":{""a"":""b"",""c"":2},""extensions"":{""d"":""e"",""f"":3}}";
            var ret = await Deserialize(request);
            ret.IsSuccessful.ShouldBeTrue();
            ret.Single.Query.ShouldBe("abc");
            ret.Single.OperationName.ShouldBe("def");
            ret.Single.Inputs["a"].ShouldBe("b");
            ret.Single.Inputs["c"].ShouldBe(2);
            ret.Single.Extensions["d"].ShouldBe("e");
            ret.Single.Extensions["f"].ShouldBe(3);
        }

        [Fact]
        public async Task Decodes_Empty_Request()
        {
            var request = @"{}";
            var ret = await Deserialize(request);
            ret.IsSuccessful.ShouldBeTrue();
            ret.Single.Query.ShouldBeNull();
            ret.Single.OperationName.ShouldBeNull();
            ret.Single.Inputs.ShouldBeNull();
            ret.Single.Extensions.ShouldBeNull();
        }

        [Fact]
        public async Task Decodes_BigInteger()
        {
            var request = @"{""variables"":{""a"":1234567890123456789012345678901234567890}}";
            var ret = await Deserialize(request);
            var bi = BigInteger.Parse("1234567890123456789012345678901234567890");
            ret.Single.Inputs["a"].ShouldBeOfType<BigInteger>().ShouldBe(bi);
        }

        [Fact]
        public async Task Dates_Should_Parse_As_Text()
        {
            var ret = await Deserialize(@"{""variables"":{""date"":""2015-12-22T10:10:10+03:00""}}");
            ret.Single.Inputs["date"].ShouldBeOfType<string>().ShouldBe("2015-12-22T10:10:10+03:00");
        }

        [Fact]
        public async Task Extensions_Null_When_Not_Provided()
        {
            var ret = await Deserialize(@"{""variables"":{""date"":""2015-12-22T10:10:10+03:00""}}");
            ret.Single.Extensions.ShouldBeNull();
        }

        [Fact]
        public async Task Name_Matching_Not_Case_Sensitive()
        {
            var ret = await Deserialize(@"{""VARIABLES"":{""date"":""2015-12-22T10:10:10+03:00""}}");
            ret.Single.Inputs["date"].ShouldBeOfType<string>().ShouldBe("2015-12-22T10:10:10+03:00");
        }

        [Fact]
        public async Task Decodes_Multiple_Queries()
        {
            var ret = await Deserialize(@"[{""query"":""abc""},{""query"":""def""}]");
            ret.Batch.Length.ShouldBe(2);
            ret.Batch[0].Query.ShouldBe("abc");
            ret.Batch[1].Query.ShouldBe("def");
        }

        [Fact]
        public async Task Decodes_Nested_Dictionaries()
        {
            var ret = await Deserialize(@"{""variables"":{""a"":{""b"":""c""}},""extensions"":{""d"":{""e"":""f""}}}");
            ret.Single.Inputs["a"].ShouldBeOfType<Dictionary<string, object>>()["b"].ShouldBe("c");
            ret.Single.Extensions["d"].ShouldBeOfType<Dictionary<string, object>>()["e"].ShouldBe("f");
        }

        [Fact]
        public async Task Decodes_Nested_Arrays()
        {
            var ret = await Deserialize(@"{""variables"":{""a"":[""b"",""c""]},""extensions"":{""d"":[""e"",""f""]}}");
            ret.Single.Inputs["a"].ShouldBeOfType<List<object>>().ShouldBe(new[] { "b", "c" });
            ret.Single.Extensions["d"].ShouldBeOfType<List<object>>().ShouldBe(new[] { "e", "f" });
        }

        private async Task<GraphQLRequestDeserializationResult> Deserialize(string jsonText)
        {
            var jsonStream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(jsonText));
            var httpRequest = new TestHttpRequest
            {
                Body = jsonStream
            };
            var deserializer = new NewtonsoftJson.GraphQLRequestDeserializer(_ => { });
            return await deserializer.DeserializeFromJsonBodyAsync(httpRequest);
        }
    }
}
