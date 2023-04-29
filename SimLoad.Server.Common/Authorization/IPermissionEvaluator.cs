using Microsoft.AspNetCore.Mvc;
using SimLoad.Server.Data;

namespace SimLoad.Server.Common.Authorization;

/// <summary>
///     Evaluates if the user has the required permissions to perform an action
///     on the specified entity whose permissions are defined by the <typeparamref name="TPermissions" /> type.
/// </summary>
/// <typeparam name="TPermissions"></typeparam>
public interface IPermissionEvaluator<TEntity, TMember, TPermissions>
    where TPermissions : class, IPermissions<TEntity>
    where TMember : class, IMember<TEntity>
{
    /// <summary>
    ///     Evaluate if the user has the required permissions to perform an action, and perform the action if they do.
    /// </summary>
    /// <param name="entityId">
    ///     The ID of the entity whose permissions are defined by <typeparamref name="TPermissions" />
    /// </param>
    /// <param name="hasPermission">Predicate to determine if the required permissions are present</param>
    /// <param name="onPermissionAllowed">Function that returns a result if the permissions are present</param>
    /// <returns>
    ///     A 404 if the entity cannot be found, 403 if the permissions requirements fail or the result of
    ///     <paramref name="onPermissionAllowed" />
    /// </returns>
    Task<IActionResult> Evaluate(Guid entityId, Func<TPermissions, bool> hasPermission,
        Func<TEntity, TMember, Task<IActionResult>> onPermissionAllowed);
}