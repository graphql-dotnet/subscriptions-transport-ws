using System.IO;
using System.Reflection;
using System.Text;

namespace GraphQL.Server.Ui.GraphiQL.Internal {

	// https://docs.microsoft.com/en-us/aspnet/core/mvc/razor-pages/?tabs=netcore-cli
	internal class GraphiQLPageModel {

		private string graphiQLCSHtml;

		private readonly GraphiQLOptions settings;

		public GraphiQLPageModel(GraphiQLOptions settings) {
			this.settings = settings;
		}

		public string Render() {
			if (graphiQLCSHtml != null) {
				return graphiQLCSHtml;
			}
			var assembly = typeof(GraphiQLPageModel).GetTypeInfo().Assembly;
            using (var manifestResourceStream = assembly.GetManifestResourceStream("GraphQL.Server.Ui.GraphiQL.Internal.graphiql.cshtml")) {
                using (var streamReader = new StreamReader(manifestResourceStream)) {
                    var builder = new StringBuilder(streamReader.ReadToEnd());
                    builder.Replace("@Model.GraphQLEndPoint", this.settings.GraphQLEndPoint);
                    builder.Replace("@Model.GraphiQLPath", this.settings.GraphiQLPath);
                    graphiQLCSHtml = builder.ToString();
                    return this.Render();
                }
            }
		}

	}

}
