import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';
import { Link } from 'react-router-dom';
import { useLogin } from '../../hooks/useAuth';

const schema = z.object({
    email: z.string()
        .min(1, 'Email je obavezan')
        .email('Unesite validan email'),
    password: z.string()
        .min(1, 'Lozinka je obavezna')
        .min(8, 'Lozinka mora imati najmanje 8 karaktera')
});

type LoginForm = z.infer<typeof schema>;

export function LoginPage() {
    const { mutate: login, isPending, error } = useLogin();

    const {
        register,
        handleSubmit,
        formState: { errors }
    } = useForm<LoginForm>({
        resolver: zodResolver(schema)
    });

    const onSubmit = (data: LoginForm) => login(data);

    return (
        <div className="min-h-screen bg-gray-50 flex items-center justify-center px-4">
            <div className="w-full max-w-md">

                <div className="text-center mb-8">
                    <span className="text-4xl"></span>
                    <h1 className="text-2xl font-bold text-gray-900 mt-2">
                        PetAdoption
                    </h1>
                    <p className="text-gray-500 mt-1">
                        Prijavi se na svoj nalog
                    </p>
                </div>

                <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">

                    <form onSubmit={handleSubmit(onSubmit)} noValidate>

                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Email adresa
                            </label>
                            <input
                                type="email"
                                placeholder="tvoj@email.com"
                                className={`w-full border rounded-lg px-3 py-2 text-sm outline-none
                                    transition-colors focus:border-orange-400 focus:ring-2 
                                    focus:ring-orange-100
                                    ${errors.email ? 'border-red-400' : 'border-gray-300'}`}
                                {...register('email')}
                            />
                            {errors.email && (
                                <p className="text-xs text-red-500 mt-1">
                                    {errors.email.message}
                                </p>
                            )}
                        </div>

                        <div className="mb-6">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                Lozinka
                            </label>
                            <input
                                type="password"
                                placeholder="••••••••"
                                className={`w-full border rounded-lg px-3 py-2 text-sm outline-none
                                    transition-colors focus:border-orange-400 focus:ring-2
                                    focus:ring-orange-100
                                    ${errors.password ? 'border-red-400' : 'border-gray-300'}`}
                                {...register('password')}
                            />
                            {errors.password && (
                                <p className="text-xs text-red-500 mt-1">
                                    {errors.password.message}
                                </p>
                            )}
                        </div>

                        {error && (
                            <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
                                <p className="text-sm text-red-600">
                                    Pogrešan email ili lozinka.
                                </p>
                            </div>
                        )}

                        <button
                            type="submit"
                            disabled={isPending}
                            className="w-full bg-orange-500 hover:bg-orange-600 text-white
                                font-medium py-2.5 rounded-lg transition-colors
                                disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            {isPending ? 'Prijavljivanje...' : 'Prijavi se'}
                        </button>
                    </form>

                    <p className="text-sm text-gray-500 text-center mt-6">
                        Nemaš nalog?{' '}
                        <Link
                            to="/register"
                            className="text-orange-500 hover:text-orange-600 font-medium"
                        >
                            Registruj se
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
}