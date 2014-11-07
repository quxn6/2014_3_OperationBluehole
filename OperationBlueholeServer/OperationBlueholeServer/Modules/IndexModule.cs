namespace OperationBlueholeServer
{
    using Nancy;

    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                return View["index"];
            };

            Get["/hi"] = parameters => "test hi";
        }
    }
}