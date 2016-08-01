using Castle.MicroKernel.Registration;
using Castle.Windsor;
using SearchEngine.Data;

namespace SearchEngine
{
    public class ContainerConfig
    {
        public static IWindsorContainer ConfigureContainer(IWindsorContainer container)
        {
            var dbContextFactory = new SearchEngineContextFactory();
            container.Register(Component.For<SearchEngineEntities>().UsingFactoryMethod(dbContextFactory.Create));
            return container;
        }
    }
}