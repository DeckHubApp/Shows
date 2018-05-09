declare namespace Rx {
    export interface ISubscription {
    }

    export class Observable<T> {
        public subscribe(next: (value: T) => void): ISubscription;
    }
}
declare namespace DeckHub.Hub {
    import Observable = Rx.Observable;

    export function subject<T>(name: string): Observable<T>;
}