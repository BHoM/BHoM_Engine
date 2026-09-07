# Pull Request #3610 Comments

Repository: `BHoM/BHoM_Engine`  
Pull request: https://github.com/BHoM/BHoM_Engine/pull/3610

## Issue comments

### Chrisshort92 — 2026-09-06T21:49:44Z

> @BHoMBot check required

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5562414517

### bhombot-ci[bot] — 2026-09-06T21:49:47Z

```
<samp>@Chrisshort92 to confirm, the following actions are now queued:

 - check `code-compliance`
 - check `documentation-compliance`
 - check `project-compliance`
 - check `core`
 - check `null-handling`
 - check `serialisation`
 - check `versioning`
 - check `installer`
</samp>
```

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5562414755

### Chrisshort92 — 2026-09-07T00:31:00Z

> The finding: the expected values in these datasets were generated under .NET Framework and are now being checked on .NET 6, and the harness stores and compares them in a way that can't absorb the difference. That bites twice. Where a test's output is a string containing a number, .NET Core 3.0 changed the default double.ToString() from 15 significant digits to shortest-round-trippable, so the identical double now prints as 6.000262860000012 instead of the stored 6.00026286000001 and the exact string comparison fails. Where the output is numeric, the harness compares with an absolute NumericTolerance = 1e-12 against mm-scale coordinates up to 7.3e6 — magnitudes at which one ULP of a double is already 9.3e-10, so the tolerance is finer than a single bit and the test effectively demands bit-exact reproduction across runtimes. 51 of the 72 numeric failures are 1–3 ULP apart, i.e. ordinary JIT/runtime variation that no code change can fix. Adding Fillet.cs is incidental: the failures reproduce byte-for-byte with it absent.
>
> The one caveat: the runtime switch explains the string diffs and the sub-3-ULP numeric ones, but not the 21 larger differences (PlaneIntersections expecting -1022.77 and getting 1.50). Those are too big to be precision artefacts — likely list ordering — and are a genuine open question for the geometry owners, just not one this PR caused.

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5563388814

### IsakNaslundBh — 2026-09-07T06:49:38Z

> quick comment on this:
>
> It is a known issue, that ofc needs to be resolved properly. In the meantime, though, if you re-create the unittests in RHino 7 (or set GH for rhino 8 to run in framework) and reserialise them, then they should be passing on the bot. To get this PR merged that will be unfortunately be required.

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5566245656

### peterjamesnugent — 2026-09-07T08:38:32Z

> Covered in the documentation here:
> https://bhom.xyz/documentation/Guides-and-Tutorials/Coding-with-BHoM/Testing/Data-Driven-Tests/#storing-test-data-for-engine-methods

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5567815305

### Chrisshort92 — 2026-09-07T10:26:00Z

> @BHoMBot check required

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5569221937

### bhombot-ci[bot] — 2026-09-07T10:26:02Z

```
<samp>@Chrisshort92 to confirm, the following actions are now queued:

 - check `code-compliance`
 - check `documentation-compliance`
 - check `project-compliance`
 - check `core`
 - check `null-handling`
 - check `serialisation`
 - check `versioning`
 - check `installer`

There are 30 requests in the queue ahead of you.</samp>
```

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5569222366

### Chrisshort92 — 2026-09-07T11:38:22Z

> @BHoMBot copyright-compliance

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570056019

### bhombot-ci[bot] — 2026-09-07T11:38:24Z

```
<samp>@Chrisshort92 sorry, I didn't understand.
Was that comment an instruction for me? If so, could you state again what check you would like me to do?
For a list of available instructions, please see [this wiki page](https://github.com/BHoM/documentation/wiki/Continuous-Integration).</samp>
```

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570056488

### Chrisshort92 — 2026-09-07T11:38:51Z

>  @BHoMBot check copyright-compliance

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570061385

### bhombot-ci[bot] — 2026-09-07T11:38:54Z

```
<samp>@Chrisshort92 to confirm, the following actions are now queued:

 - check `copyright-compliance`

There are 9 requests in the queue ahead of you.</samp>
```

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570061869

### Chrisshort92 — 2026-09-07T11:39:32Z

> @BHoMBot check dataset-compliance

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570068687

### bhombot-ci[bot] — 2026-09-07T11:39:34Z

```
<samp>@Chrisshort92 to confirm, the following actions are now queued:

 - check `dataset-compliance`

There are 9 requests in the queue ahead of you.</samp>
```

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570069205

### Chrisshort92 — 2026-09-07T11:41:35Z

> @BHoMBot check unit-tests

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570091409

### bhombot-ci[bot] — 2026-09-07T11:41:38Z

```
<samp>@Chrisshort92 to confirm, the following actions are now queued:

 - check `unit-tests`

There are 11 requests in the queue ahead of you.</samp>
```

https://github.com/BHoM/BHoM_Engine/pull/3610#issuecomment-5570091811

## Review bodies

### Martian42 — APPROVED — 2026-09-07T10:46:55Z

```
<img width="937" height="971" alt="Image" src="https://github.com/user-attachments/assets/57de9e04-d91c-4fc4-b54f-313b8e73e54b" />
Successfully replicate the intended output using the test script. Happy to merge.
```

https://github.com/BHoM/BHoM_Engine/pull/3610#pullrequestreview-5131054501

### peterjamesnugent — CHANGES_REQUESTED — 2026-09-07T20:48:56Z

```
Comments below.

In your test script this fillet is only 1.719 whilst the input is 1.83:
<img width="1197" height="761" alt="Image" src="https://github.com/user-attachments/assets/151bbf19-104a-49f9-9e43-74da6d194c44" />

Can you clean up the comments in the file please - some seem like reasoning and should be cleaned up for merging.

Also, does your test script/unit test cover most cases? Is there one for covering where the radius is greater than the line segment? What about short segments where the points are near co-incident?
```

https://github.com/BHoM/BHoM_Engine/pull/3610#pullrequestreview-5131316527

## Review comments

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:40 — 2026-09-07T11:13:56Z

> > reduces where the trim-length cap bites.
>
> Can we reword this to be clearer and more verbose.

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3949166383

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:96 — 2026-09-07T15:02:46Z

> Would this not be better as an input with a default value? 

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3950892125

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:123 — 2026-09-07T15:47:45Z

> Could you not just check for `closed` at the start and remove the last `pt`?
>
> Your `for` then covers anything that is too close.

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3951181608

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:81 — 2026-09-07T15:50:17Z

> Is this comment still relevant given what you do in `FilletVertices`?

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3951196780

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:140 — 2026-09-07T16:16:59Z

> Mighth be worth a comment what is going on here, especially the `(s + 1) % nVerts`

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3951348377

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:42 — 2026-09-07T20:04:23Z

```suggestion
        [Input("radius", "Target fillet radius (> 0). The radius achieved at a corner reduces where the trim-length cap bites.", typeof(Length))]
        [Input("distTol", "Distance tolerance.", typeof(Length))]
        [Input("angleTol", "Angle tolerance. A joint is left sharp where it comes within this of running straight through, or of doubling back on itself.", typeof(Angle))]
```

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952363864

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:159 — 2026-09-07T20:14:22Z

> You could just normalise here:
> https://github.com/BHoM/BHoM_Engine/blob/acc84d2c835c23cdd24d515c9c2df55fb4dc1333/Geometry_Engine/Modify/Normalise.cs#L41

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952404690

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:168 — 2026-09-07T20:16:08Z

> I think this logic - along with the unitised vector above can be replaced with the `Query.Angle` method:
> https://github.com/BHoM/BHoM_Engine/blob/acc84d2c835c23cdd24d515c9c2df55fb4dc1333/Geometry_Engine/Query/Angle.cs#L40-L52

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952411190

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:265 — 2026-09-07T20:16:31Z

> Similar to above, you can use `Query.Angle` here:
> https://github.com/BHoM/BHoM_Engine/blob/acc84d2c835c23cdd24d515c9c2df55fb4dc1333/Geometry_Engine/Query/Angle.cs#L40-L52

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952412565

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:238 — 2026-09-07T20:22:17Z

> Is it necessary to have an additional method that just calls `FilletArc`? Can't you just implement that in a `for` loop instead?

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952434802

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:106 — 2026-09-07T20:39:47Z

> Is this still valid?

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952501838

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:38 — 2026-09-07T20:40:54Z

> This can be broken with `\n` for readability.

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952505926

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:92 — 2026-09-07T20:42:05Z

```suggestion
        /***************************************************/
        /**** Private Fields                             ****/
        /***************************************************/
```

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952510375

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:323 — 2026-09-07T20:42:44Z

```suggestion
        }
/***************************************************/

```

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952513622

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:278 — 2026-09-07T20:44:24Z

```suggestion
            Vector bisector = (v1 + v2).Normalise();
```

Can we be verbose with the names to add context when reviewing later.

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952519496

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:273 — 2026-09-07T20:44:56Z

```suggestion
            double r = trim * Math.Tan(theta / 2.0);
```

Variable names should be lower case - what is r in this context?

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952521362

### peterjamesnugent — Geometry_Engine/Modify/Fillet.cs:176 — 2026-09-07T20:46:05Z

> What is t in this context? I would rename t to a clearer variable name or add a comment. Giving it a clearer variable name is preferable.

https://github.com/BHoM/BHoM_Engine/pull/3610#discussion_r3952526027
