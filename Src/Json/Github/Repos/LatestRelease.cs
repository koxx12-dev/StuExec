#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8765 // Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
namespace StuExec.Json.Github.Repos;

using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

//Generated with https://app.quicktype.io/

/// <summary>
/// A release.
/// </summary>
public class LatestRelease
{
    [JsonProperty("assets")] public ReleaseAsset[] Assets { get; set; }

    [JsonProperty("assets_url")] public Uri AssetsUrl { get; set; }

    /// <summary>
    /// A GitHub user.
    /// </summary>
    [JsonProperty("author")]
    public AuthorClass Author { get; set; }

    [JsonProperty("body")] public string Body { get; set; }

    [JsonProperty("body_html", NullValueHandling = NullValueHandling.Ignore)]
    public string BodyHtml { get; set; }

    [JsonProperty("body_text", NullValueHandling = NullValueHandling.Ignore)]
    public string BodyText { get; set; }

    [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The URL of the release discussion.
    /// </summary>
    [JsonProperty("discussion_url", NullValueHandling = NullValueHandling.Ignore)]
    public Uri DiscussionUrl { get; set; }

    /// <summary>
    /// true to create a draft (unpublished) release, false to create a published one.
    /// </summary>
    [JsonProperty("draft")]
    public bool Draft { get; set; }

    [JsonProperty("html_url")] public Uri HtmlUrl { get; set; }

    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("mentions_count", NullValueHandling = NullValueHandling.Ignore)]
    public long? MentionsCount { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("node_id")] public string NodeId { get; set; }

    /// <summary>
    /// Whether to identify the release as a prerelease or a full release.
    /// </summary>
    [JsonProperty("prerelease")]
    public bool Prerelease { get; set; }

    [JsonProperty("published_at")] public DateTimeOffset? PublishedAt { get; set; }

    [JsonProperty("reactions", NullValueHandling = NullValueHandling.Ignore)]
    public ReactionRollup Reactions { get; set; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    [JsonProperty("tag_name")]
    public string TagName { get; set; }

    [JsonProperty("tarball_url")] public Uri TarballUrl { get; set; }

    /// <summary>
    /// Specifies the commitish value that determines where the Git tag is created from.
    /// </summary>
    [JsonProperty("target_commitish")]
    public string TargetCommitish { get; set; }

    [JsonProperty("upload_url")] public string UploadUrl { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }

    [JsonProperty("zipball_url")] public Uri ZipballUrl { get; set; }
}

/// <summary>
/// Data related to a release.
/// </summary>
public class ReleaseAsset
{
    [JsonProperty("browser_download_url")] public Uri BrowserDownloadUrl { get; set; }

    [JsonProperty("content_type")] public string ContentType { get; set; }

    [JsonProperty("created_at")] public DateTimeOffset CreatedAt { get; set; }

    [JsonProperty("download_count")] public long DownloadCount { get; set; }

    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("label")] public string Label { get; set; }

    /// <summary>
    /// The file name of the asset.
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("node_id")] public string NodeId { get; set; }

    [JsonProperty("size")] public long Size { get; set; }

    /// <summary>
    /// State of the release asset.
    /// </summary>
    [JsonProperty("state")]
    public State State { get; set; }

    [JsonProperty("updated_at")] public DateTimeOffset UpdatedAt { get; set; }

    [JsonProperty("uploader")] public SimpleUser Uploader { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }
}

/// <summary>
/// A GitHub user.
/// </summary>
public class SimpleUser
{
    [JsonProperty("avatar_url")] public Uri AvatarUrl { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("events_url")] public string EventsUrl { get; set; }

    [JsonProperty("followers_url")] public Uri FollowersUrl { get; set; }

    [JsonProperty("following_url")] public string FollowingUrl { get; set; }

    [JsonProperty("gists_url")] public string GistsUrl { get; set; }

    [JsonProperty("gravatar_id")] public string GravatarId { get; set; }

    [JsonProperty("html_url")] public Uri HtmlUrl { get; set; }

    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("login")] public string Login { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("node_id")] public string NodeId { get; set; }

    [JsonProperty("organizations_url")] public Uri OrganizationsUrl { get; set; }

    [JsonProperty("received_events_url")] public Uri ReceivedEventsUrl { get; set; }

    [JsonProperty("repos_url")] public Uri ReposUrl { get; set; }

    [JsonProperty("site_admin")] public bool SiteAdmin { get; set; }

    [JsonProperty("starred_at", NullValueHandling = NullValueHandling.Ignore)]
    public string StarredAt { get; set; }

    [JsonProperty("starred_url")] public string StarredUrl { get; set; }

    [JsonProperty("subscriptions_url")] public Uri SubscriptionsUrl { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }
}

/// <summary>
/// A GitHub user.
/// </summary>
public class AuthorClass
{
    [JsonProperty("avatar_url")] public Uri AvatarUrl { get; set; }

    [JsonProperty("email")] public string Email { get; set; }

    [JsonProperty("events_url")] public string EventsUrl { get; set; }

    [JsonProperty("followers_url")] public Uri FollowersUrl { get; set; }

    [JsonProperty("following_url")] public string FollowingUrl { get; set; }

    [JsonProperty("gists_url")] public string GistsUrl { get; set; }

    [JsonProperty("gravatar_id")] public string GravatarId { get; set; }

    [JsonProperty("html_url")] public Uri HtmlUrl { get; set; }

    [JsonProperty("id")] public long Id { get; set; }

    [JsonProperty("login")] public string Login { get; set; }

    [JsonProperty("name")] public string Name { get; set; }

    [JsonProperty("node_id")] public string NodeId { get; set; }

    [JsonProperty("organizations_url")] public Uri OrganizationsUrl { get; set; }

    [JsonProperty("received_events_url")] public Uri ReceivedEventsUrl { get; set; }

    [JsonProperty("repos_url")] public Uri ReposUrl { get; set; }

    [JsonProperty("site_admin")] public bool SiteAdmin { get; set; }

    [JsonProperty("starred_at", NullValueHandling = NullValueHandling.Ignore)]
    public string StarredAt { get; set; }

    [JsonProperty("starred_url")] public string StarredUrl { get; set; }

    [JsonProperty("subscriptions_url")] public Uri SubscriptionsUrl { get; set; }

    [JsonProperty("type")] public string Type { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }
}

public class ReactionRollup
{
    [JsonProperty("+1")] public long The1 { get; set; }

    [JsonProperty("-1")] public long ReactionRollup1 { get; set; }

    [JsonProperty("confused")] public long Confused { get; set; }

    [JsonProperty("eyes")] public long Eyes { get; set; }

    [JsonProperty("heart")] public long Heart { get; set; }

    [JsonProperty("hooray")] public long Hooray { get; set; }

    [JsonProperty("laugh")] public long Laugh { get; set; }

    [JsonProperty("rocket")] public long Rocket { get; set; }

    [JsonProperty("total_count")] public long TotalCount { get; set; }

    [JsonProperty("url")] public Uri Url { get; set; }
}

/// <summary>
/// State of the release asset.
/// </summary>
public enum State
{
    Open,
    Uploaded
};

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new()
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            StateConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        }
    };
}

internal class StateConverter : JsonConverter
{
    public override bool CanConvert(Type t)
    {
        return t == typeof(State) || t == typeof(State?);
    }

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null) return null;
        var value = serializer.Deserialize<string>(reader);
        switch (value)
        {
            case "open":
                return State.Open;
            case "uploaded":
                return State.Uploaded;
        }

        throw new Exception("Cannot unmarshal type State");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (untypedValue == null)
        {
            serializer.Serialize(writer, null);
            return;
        }

        var value = (State)untypedValue;
        switch (value)
        {
            case State.Open:
                serializer.Serialize(writer, "open");
                return;
            case State.Uploaded:
                serializer.Serialize(writer, "uploaded");
                return;
        }

        throw new Exception("Cannot marshal type State");
    }

    public static readonly StateConverter Singleton = new();
}