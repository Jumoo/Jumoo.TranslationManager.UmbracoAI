# Security Policy

## Supported versions

The Umbraco AI connector ships one release line per Umbraco major, matching the Translation
Manager release it plugs into. Fixes go to the current line, and to the previous one where the
issue is serious and the fix is practical.

| Version | Branch | Supported |
| --- | --- | --- |
| 17.x | `main` | Yes |
| Earlier | — | No |

## Reporting a vulnerability

Please **do not** open a public issue for a security problem.

Email **kevin@jumoo.co.uk** with a description of the issue, the version affected, and steps to
reproduce it. We'll acknowledge within a few working days and keep you updated as we work on it.

This connector bridges Translation Manager to Umbraco.AI's chat service and profile
configuration, so if the issue involves how a profile ID or API credential is resolved,
passed to a provider, or logged, say so — those get sequenced first.
