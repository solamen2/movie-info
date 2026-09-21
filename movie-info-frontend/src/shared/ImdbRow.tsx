import { type ReactNode, useEffect, useRef, useState } from "react";
import "./shared.css";

const COPY_FEEDBACK_MS = 1500;

export function ImdbRowSeparator() {
  return (
    <span
      className="imdb-row-separator"
      role="separator"
      aria-orientation="vertical"
    />
  );
}

interface ImdbRowProps {
  imdbUrl: string;
  // The IMDB data shown ahead of the link (rating, rank, etc.)
  children: ReactNode;
}

function ImdbRow({ imdbUrl, children }: ImdbRowProps) {
  const [copyStatus, setCopyStatus] = useState<"idle" | "copied" | "failed">(
    "idle",
  );
  const copyTimeoutRef = useRef<number | undefined>(undefined);

  useEffect(() => {
    return () => {
      window.clearTimeout(copyTimeoutRef.current);
    };
  }, []);

  async function handleCopy() {
    window.clearTimeout(copyTimeoutRef.current);
    try {
      await navigator.clipboard.writeText(`<a href="${imdbUrl}">Link</a>`);
      setCopyStatus("copied");
    } catch {
      setCopyStatus("failed");
    }
    copyTimeoutRef.current = window.setTimeout(() => {
      setCopyStatus("idle");
    }, COPY_FEEDBACK_MS);
  }

  return (
    <p className="imdb-row">
      <span>
        {children}
        <ImdbRowSeparator />
        <a href={imdbUrl} target="_blank" rel="noopener">
          Link
        </a>
      </span>
      <button
        type="button"
        aria-label="copy-imdb-link"
        className="copy-button"
        onClick={handleCopy}
      >
        {copyStatus === "copied"
          ? "Copied!"
          : copyStatus === "failed"
            ? "Failed"
            : "Copy"}
      </button>
    </p>
  );
}

export default ImdbRow;
