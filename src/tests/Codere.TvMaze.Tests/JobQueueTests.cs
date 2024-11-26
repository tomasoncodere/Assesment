using Codere.TvMaze.Application.Features.SyncTvShows;
using Codere.TvMaze.Infrastructure.Jobs;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Codere.TvMaze.Infrastructure.Tests;

public class JobQueueTests
{
    [Fact]
    public void Dispatch_ShouldEnqueueJob_WhenChannelIsOpen()
    {
        // Arrange
        var logger = Substitute.For<ILogger<JobQueue>>();
        var sut = new JobQueue(logger);

        var job = Substitute.For<IJob>();

        // Act
        var result = sut.Dispatch(job);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Dispatch_ShouldReturnFalse_WhenChannelIsClosed()
    {
        // Arrange
        var logger = Substitute.For<ILogger<JobQueue>>();
        var sut = new JobQueue(logger);


        await sut.StartAsync(CancellationToken.None);
        await sut.StopAsync(CancellationToken.None);

        var job = Substitute.For<IJob>();

        // Act
        var result = sut.Dispatch(job);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task StopAsync_ShouldCompleteChannelAndFinishProcessing()
    {
        // Arrange
        var sut = new JobQueue(Substitute.For<ILogger<JobQueue>>());
        var jobCompletionSource = new TaskCompletionSource(); 

        var job = Substitute.For<IJob>();
        job.Execute().Returns(jobCompletionSource.Task); 

        await sut.StartAsync(CancellationToken.None); 

        // Act
        sut.Dispatch(job);

        var stopTask = Task.Run(async () => await sut.StopAsync(CancellationToken.None), CancellationToken.None);

        stopTask.IsCompleted.Should().BeFalse();

        await Task.Delay(1000, CancellationToken.None);
        jobCompletionSource.SetResult();

        await stopTask;

        // Assert
        await job.Received(1).Execute();
        stopTask.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldProcessMultipleJobsInOrder()
    {
        // Arrange
        var logger = Substitute.For<ILogger<JobQueue>>();
        var jobQueue = new JobQueue(logger);

        var job1 = Substitute.For<IJob>();
        var job2 = Substitute.For<IJob>();

        job1.Execute().Returns(Task.CompletedTask);
        job2.Execute().Returns(Task.CompletedTask);

        jobQueue.Dispatch(job1);
        jobQueue.Dispatch(job2);

        // Act
        var cts = new CancellationTokenSource();
        cts.CancelAfter(300);
        await jobQueue.StartAsync(cts.Token);

        // Assert
        Received.InOrder(() =>
        {
            job1.Execute();

            job2.Execute();
        });
    }
}