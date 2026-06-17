const POWER_PASS_LENGTH = 20
const CHARSET_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
const CHARSET_LOWER = "abcdefghijklmnopqrstuvwxyz"
const CHARSET_DIGITS = "0123456789"
const CHARSET_SYMBOLS = "!@#$%^&*"
const CHARSET_ALL = CHARSET_UPPER + CHARSET_LOWER + CHARSET_DIGITS + CHARSET_SYMBOLS

function pickSecureChar(pool: string): string {
  const max = 256 - (256 % pool.length)
  const buf = new Uint8Array(1)
  for (;;) {
    crypto.getRandomValues(buf)
    if (buf[0] < max) {
      return pool[buf[0] % pool.length]!
    }
  }
}

function shuffleInPlace(arr: string[]): void {
  for (let i = arr.length - 1; i > 0; i--) {
    const jBuf = new Uint32Array(1)
    crypto.getRandomValues(jBuf)
    const j = jBuf[0]! % (i + 1)
    ;[arr[i], arr[j]] = [arr[j]!, arr[i]!]
  }
}

/** 20-char password: upper, lower, digit, symbol, crypto-random (rejection sampling). */
export function generatePowerPass(length: number = POWER_PASS_LENGTH): string {
  const required = [
    pickSecureChar(CHARSET_UPPER),
    pickSecureChar(CHARSET_LOWER),
    pickSecureChar(CHARSET_DIGITS),
    pickSecureChar(CHARSET_SYMBOLS),
  ]
  const rest: string[] = []
  for (let n = 0; n < length - 4; n++) {
    rest.push(pickSecureChar(CHARSET_ALL))
  }
  const chars = [...required, ...rest]
  shuffleInPlace(chars)
  return chars.join("")
}
