# Reminders for me:

Immediately after updating a feature branch, run this in your terminal to catch any styling issues before you push:

```
dotnet format --verify-no-changes
```

If it flags any style errors automatically fix them locally by running:

```
dotnet format
```
