/* Runguard config for use with CodeRunner. Includes all necessary
 * DOMJudge constants from config.h.
 * Assumes all tests will be done as the Linux user "coderunner".
 * It is assumed CHROOT will not be used so the CHROOT_PREFIX
 * is not meaningfully set.
 */

#ifndef _RUNGUARD_CONFIG_
#define _RUNGUARD_CONFIG_

#define DOMJUDGE_VERSION "3"
#define REVISION "3.3"

#define VALID_USERS "domjudge"

#define CHROOT_PREFIX "/var/www/jobe"

#endif /* _RUNGUARD_CONFIG_ */