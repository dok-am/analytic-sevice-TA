using Zenject;
using Systems.Analytic;

public class ServicesInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AnalyticService>().FromNewComponentOn(gameObject).AsSingle().NonLazy();
    }
}