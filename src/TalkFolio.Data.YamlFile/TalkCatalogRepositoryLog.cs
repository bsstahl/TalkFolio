namespace TalkFolio.Data.YamlFile;

using Microsoft.Extensions.Logging;

internal static partial class TalkCatalogRepositoryLog
{
    [LoggerMessage(EventId = 1, Level = LogLevel.Information, Message = "Loading TalkFolio catalog.")]
    public static partial void LoadingCatalog(ILogger logger);

    [LoggerMessage(EventId = 2, Level = LogLevel.Error, Message = "TalkFolio catalog load failed because DataRoot is not configured.")]
    public static partial void LoadingCatalogFailedBecauseDataRootNotConfigured(ILogger logger);

    [LoggerMessage(EventId = 3, Level = LogLevel.Error, Message = "TalkFolio catalog load failed because DataRoot does not exist: {DataRoot}")]
    public static partial void LoadingCatalogFailedBecauseDataRootDoesNotExist(ILogger logger, string dataRoot);

    [LoggerMessage(EventId = 4, Level = LogLevel.Information, Message = "Loading talks from {Directory}.")]
    public static partial void LoadingTalksFrom(ILogger logger, string directory);

    [LoggerMessage(EventId = 5, Level = LogLevel.Information, Message = "Loaded TalkFolio catalog with {TalkCount} talks.")]
    public static partial void LoadedCatalog(ILogger logger, int talkCount);

    [LoggerMessage(EventId = 6, Level = LogLevel.Warning, Message = "Talks directory does not exist: {Directory}")]
    public static partial void TalksDirectoryDoesNotExist(ILogger logger, string directory);

    [LoggerMessage(EventId = 7, Level = LogLevel.Trace, Message = "Reading talk file {FilePath}.")]
    public static partial void ReadingTalkFile(ILogger logger, string filePath);

    [LoggerMessage(EventId = 8, Level = LogLevel.Trace, Message = "Deserialized talk payload {TalkId} ({TalkTitle}) from {FilePath}.")]
    public static partial void DeserializedTalkPayload(ILogger logger, Guid talkId, string talkTitle, string filePath);

    [LoggerMessage(EventId = 9, Level = LogLevel.Trace, Message = "Mapped talk {TalkId} from {FilePath}.")]
    public static partial void MappedTalk(ILogger logger, Guid talkId, string filePath);

    [LoggerMessage(EventId = 10, Level = LogLevel.Error, Message = "Catalog load failed because talk file {FilePath} contains malformed YAML.")]
    public static partial void TalkFileMalformed(ILogger logger, Exception exception, string filePath);

    [LoggerMessage(EventId = 11, Level = LogLevel.Error, Message = "Catalog load failed because talk file {FilePath} could not be deserialized.")]
    public static partial void TalkFileCouldNotBeDeserialized(ILogger logger, Exception exception, string filePath);

    [LoggerMessage(EventId = 12, Level = LogLevel.Error, Message = "Catalog load failed because duplicate TalkId {TalkId} was found in {DuplicateFilePath}. First seen in {FirstFilePath}.")]
    public static partial void DuplicateTalkId(ILogger logger, Exception exception, Guid talkId, string duplicateFilePath, string firstFilePath);

    [LoggerMessage(EventId = 13, Level = LogLevel.Error, Message = "Catalog load failed because duplicate talk title and variant were found for Title '{Title}' and Variant '{Variant}' in {DuplicateFilePath}. First seen in {FirstFilePath}.")]
    public static partial void DuplicateTalkTitleVariant(ILogger logger, Exception exception, string title, string variant, string duplicateFilePath, string firstFilePath);

    [LoggerMessage(EventId = 14, Level = LogLevel.Error, Message = "Catalog load failed because talk file {FilePath} is missing required Id.")]
    public static partial void TalkFileMissingRequiredId(ILogger logger, Exception exception, string filePath);
}