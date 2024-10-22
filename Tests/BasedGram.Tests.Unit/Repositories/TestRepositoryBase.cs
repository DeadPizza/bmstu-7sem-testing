namespace BasedGram.Tests.Unit.Repositories;

public class TestRepositoryBase
{
    protected readonly MockDbContextFactory m_mockDbContextFactory;
    public TestRepositoryBase()
    {
        m_mockDbContextFactory = new MockDbContextFactory();
    }
}
