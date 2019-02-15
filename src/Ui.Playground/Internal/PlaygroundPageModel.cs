using System.IO;
using System.Reflection;
using System.Text;

namespace GraphQL.Server.Ui.Playground.Internal {

	// https://docs.microsoft.com/en-us/aspnet/core/mvc/razor-pages/?tabs=netcore-cli
	internal class PlaygroundPageModel {

		private string playgroundCSHtml;

		private readonly GraphQLPlaygroundOptions settings;

		public PlaygroundPageModel(GraphQLPlaygroundOptions settings) {
			this.settings = settings;
		}

		public string Render() {
			if (playgroundCSHtml != null) {
				return playgroundCSHtml;
			}
			var assembly = typeof(PlaygroundPageModel).GetTypeInfo().Assembly;
            using (var manifestResourceStream = assembly.GetManifestResourceStream("GraphQL.Server.Ui.Playground.Internal.playground.cshtml")) {
                using (var streamReader = new StreamReader(manifestResourceStream)) {
                    var builder = new StringBuilder(streamReader.ReadToEnd());
                    builder.Replace("@Model.GraphQLEndPoint", this.settings.GraphQLEndPoint);
                    builder.Replace("@Model.PlaygroundPath", this.settings.Path);
                    playgroundCSHtml = builder.ToString();
                    return this.Render();
                }
            }
		}

	}

}
