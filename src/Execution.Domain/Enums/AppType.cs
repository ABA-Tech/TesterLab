namespace Execution.Domain.Enums;

public enum AppType { Web, Mobile, Api, Desktop }

public enum EnvironmentType { Development, Staging, Production, Testing }

public enum TestCaseType { E2E, Api, Performance }

public enum ExecutionFrequency { Manual, Daily, Weekly, OnDemand }

public enum Complexity { Low, Medium, High }

public enum TriggerType { Manual, Scheduled, CiCd, Api, Rerun }

public enum ExecutionType { TestCase, Feature, Multiple, FullRegression }

public enum BrowserType { Chrome, Firefox, Safari, Edge, WebKit }

public enum RunStatus { Created, Queued, Running, Passed, Failed, PartiallyFailed, Cancelled, TimedOut }

public enum ExecutionStatus { Pending, Running, Passed, Failed, Skipped, Error }

public enum LogLevel { Debug, Info, Warning, Error }

public enum ScreenshotType { Success, Failure, Error, Assertion, Manual }

public enum DataType { String, Integer, Boolean, Json }

public enum SettingCategory { General, Email, Testing, Security, Branding, Notifications, Storage }