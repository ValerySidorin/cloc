namespace Cloc;

public interface IClocJobFactory
{
    IClocJob NewJob(ClocJobOptions options);

    void AddJob<TClocJob>(TClocJob job = default)
        where TClocJob : class, IClocJob;
}
